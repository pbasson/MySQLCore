namespace MySQLCore.Worker.BackgroundServices.ImageGallery;

public sealed class RabbitMQImageWorker : BaseWorker<ImageGalleryMessage>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMQBus _messageBus;
    public readonly RabbitMQSetting _settings;
    public readonly IRabbitMQConnection _messageConnection;

    public RabbitMQImageWorker(ILogger<RabbitMQImageWorker> logger, IServiceScopeFactory scopeFactory, IOptions<RabbitMQSetting> options,
        IRabbitMQConnection messageConnection, IRabbitMQBus messageBus) : base(logger)
    {
        _scopeFactory = scopeFactory;
        _messageBus = messageBus;
        _settings = options.Value;
        _messageConnection = messageConnection;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            int retrying = 5;

            try
            {
                await using IChannel channel = await _messageConnection.CreateChannelAsync(cancellationToken);
                var restart = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                channel.ChannelShutdownAsync += (_, _) =>
                {
                    restart.TrySetResult(true);
                    return Task.CompletedTask;
                };

                // Limit deliveries held by this consumer while processing or recovering.
                await channel.BasicQosAsync(0, 1, false, cancellationToken);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.UnregisteredAsync += (_, _) =>
                {
                    restart.TrySetResult(true);
                    return Task.CompletedTask;
                };
                consumer.ReceivedAsync += async (_, args) =>
                {
                    try
                    {
                        await HandleMessageAsync(args, channel, cancellationToken);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        // Channel disposal during shutdown returns unacknowledged deliveries.
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Delivery forwarding/acknowledgement failed. Restarting consumer. DeliveryTag: {DeliveryTag}",
                            args.DeliveryTag);
                        // Do not acknowledge or retry processing here: the outcome may be uncertain.
                        restart.TrySetResult(true);
                    }
                };

                await channel.BasicConsumeAsync(queue: MessagerConstants.IMAGE_QUEUE, autoAck: false,
                    consumer, cancellationToken);
                await restart.Task.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Image consumer unavailable. Retrying in {retry} seconds.", retrying);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(retrying), cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
    }

    private async Task HandleMessageAsync(BasicDeliverEventArgs args, IChannel channel, CancellationToken cancellationToken)
    {
        // One activity covers validation, processing, retry/dead-letter forwarding and acknowledgement.
        using Activity? activity = TracingConstants.MessagingActivitySource.StartActivity(
            $"{nameof(RabbitMQImageWorker)}.{nameof(HandleMessageAsync)}", ActivityKind.Consumer);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination.name", MessagerConstants.IMAGE_QUEUE);
        activity?.SetTag("message.type", nameof(ImageGalleryMessage));
        activity?.SetTag("DeliveryTag", args.DeliveryTag);

        try
        {
            MessageMetric.Received.Inc();
            ImageGalleryMessage? message;
            try
            {
                message = DeserializeMessage(args.Body);
                
                if (message == null || message.MessageId == Guid.Empty)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, "Missing message or MessageId");
                    await DeadLetterInvalidAsync(args, channel, cancellationToken);
                    return;
                }
            }
            catch (JsonException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Malformed image payload");
                _logger.LogWarning(ex, "Malformed image message. DeliveryTag: {DeliveryTag}", args.DeliveryTag);
                await DeadLetterInvalidAsync(args, channel, cancellationToken);
                return;
            }

            activity?.SetTag("message.id", message.MessageId);
            activity?.SetTag("image.id", message.ImageId);

            try
            {
                await ProcessMessage(message, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Image processing failed");
                await ProcessMessageException(args, channel, message, ex, cancellationToken);
                return;
            }

            // Database processing has completed; acknowledgement failure is a delivery problem.
            await BasicAckAsync(args, channel, cancellationToken);
            _logger.LogInformation("Acknowledgement sent. MessageId: {MessageId}", message.MessageId);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Expected shutdown is not a processing failure.
            throw;
        }
        catch (Exception ex)
        {
            // Includes forwarding/acknowledgement failures; preserve the existing restart behaviour.
            activity?.SetStatus(ActivityStatusCode.Error, "Message delivery failed");
            activity?.SetTag("error.type", ex.GetType().FullName);
            throw;
        }
    }

    private async Task DeadLetterInvalidAsync(BasicDeliverEventArgs args, IChannel channel, CancellationToken cancellationToken)
    {
        MessageMetric.Failed.Inc();
        await ForwardAndAckAsync(_settings.DeadLetterQueueName, args, channel, args.BasicProperties.Headers, cancellationToken);
        MessageMetric.DeadLetter.Inc();
        _logger.LogWarning("Invalid payload forwarded to dead-letter queue. DeliveryTag: {DeliveryTag}", args.DeliveryTag);
    }

    private async Task ProcessMessage(ImageGalleryMessage message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var processService = scope.ServiceProvider.GetRequiredService<ProcessWorkerService>();

        var result = await processService.ProcessAsync(message, cancellationToken);

        if(result == ProcessWorkerResult.Duplicate) return;

        _logger.LogInformation( "{messager} Message Status: {Status}, MessageId: {MessageId}", 
            nameof(ImageGalleryMessage), nameof(ProcessMessageStatus.Processed), message.MessageId);
        MessageMetric.Processed.Inc();

    }

    private async Task ProcessMessageException(BasicDeliverEventArgs args, IChannel channel, ImageGalleryMessage? message, Exception ex, CancellationToken cancellationToken)
    {
        _logger.LogError(ex, "{messager} Message Status: {status}, DeliveryTag: {DeliveryTag}", nameof(ImageGalleryMessage),
            nameof(ProcessMessageStatus.Failed), args.DeliveryTag);
        MessageMetric.Failed.Inc();

        var retryCount = GetRetryCount(args);

        var headers = args.BasicProperties.Headers == null
            ? new Dictionary<string, object?>() : new Dictionary<string, object?>(args.BasicProperties.Headers);

        if (retryCount >= _settings.MaxRetryCount)
        {
            await ForwardAndAckAsync(_settings.DeadLetterQueueName, args, channel, headers, cancellationToken);
            _logger.LogWarning("Delivery forwarded to dead-letter queue. MessageId: {MessageId}", message?.MessageId);
            MessageMetric.DeadLetter.Inc();
            return;
        }

        headers[_settings.RetryHeader] = retryCount + 1;
        await ForwardAndAckAsync(_settings.RetryQueueName, args, channel, headers, cancellationToken);

        return; 
    }

    private async Task BasicAckAsync(BasicDeliverEventArgs args, IChannel channel, CancellationToken cancellationToken)
    {
        await channel.BasicAckAsync(deliveryTag: args.DeliveryTag, multiple: false, cancellationToken);
        MessageMetric.Acknowledged.Inc();
    }

    private async Task ForwardAndAckAsync(string destinationQueue, BasicDeliverEventArgs args, IChannel channel,
        IDictionary<string, object?>? headers, CancellationToken cancellationToken)
    {
        await _messageBus.ForwardAsync(destinationQueue, args.Body, headers, cancellationToken);
        await BasicAckAsync(args, channel, cancellationToken);
    }

    private int GetRetryCount(BasicDeliverEventArgs args)
    {
        if (args.BasicProperties?.Headers == null 
            || !args.BasicProperties.Headers.TryGetValue(_settings.RetryHeader, out var value))
            return 0;

        return value switch
        {
            byte[] bytes when int.TryParse(Encoding.UTF8.GetString(bytes), out var result) => result,
            int number => number,
            long number => (int)number,
            _ => 0
        };
    }
}
