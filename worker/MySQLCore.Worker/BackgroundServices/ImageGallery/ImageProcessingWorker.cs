namespace MySQLCore.Worker.BackgroundServices.ImageGallery;

public sealed class ImageProcessingWorker : BaseWorker<ImageCreatedMessage>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessagePublisher _publisher;
    
    public ImageProcessingWorker(ILogger<ImageProcessingWorker> logger, IServiceScopeFactory scopeFactory, IOptions<RabbitMQSettings> options, RabbitMQConnectionService connectionService, IMessagePublisher publisher)
        : base(logger, options, connectionService)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using IChannel channel = await _connectionService.CreateChannelAsync(stoppingToken);
                var restart = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                channel.ChannelShutdownAsync += (_, _) =>
                {
                    restart.TrySetResult(true);
                    return Task.CompletedTask;
                };

                // Limit deliveries held by this consumer while processing or recovering.
                await channel.BasicQosAsync(0, 1, false, stoppingToken);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.UnregisteredAsync += (_, _) =>
                {
                    restart.TrySetResult(true);
                    return Task.CompletedTask;
                };
                consumer.ReceivedAsync += async (_, eventArgs) =>
                {
                    try
                    {
                        await HandleDeliveryAsync(eventArgs, channel, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // Channel disposal during shutdown returns unacknowledged deliveries.
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Delivery forwarding or acknowledgement failed. Restarting consumer. DeliveryTag: {DeliveryTag}",
                            eventArgs.DeliveryTag);
                        // Do not acknowledge or retry processing here: the outcome may be uncertain.
                        restart.TrySetResult(true);
                    }
                };

                await channel.BasicConsumeAsync(queue: MessagerConstants.IMAGE_QUEUE, autoAck: false,
                    consumer: consumer, cancellationToken: stoppingToken);
                await restart.Task.WaitAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Image consumer unavailable. Retrying in 5 seconds.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
        }
    }

    private async Task HandleDeliveryAsync(BasicDeliverEventArgs eventArgs, IChannel channel, CancellationToken stoppingToken)
    {
        MessageMetrics.Received.Inc();
        ImageCreatedMessage? message;
        try
        {
            message = DeserializeMessage(eventArgs);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Malformed image message. DeliveryTag: {DeliveryTag}", eventArgs.DeliveryTag);
            await DeadLetterInvalidAsync(eventArgs, channel, stoppingToken);
            return;
        }

        if (message == null || message.MessageId == Guid.Empty)
        {
            await DeadLetterInvalidAsync(eventArgs, channel, stoppingToken);
            return;
        }

        try
        {
            await ProcessMessage(message, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            await ProcessMessageException(eventArgs, channel, message, ex, stoppingToken);
            return;
        }

        // Database processing has completed; acknowledgement failure is a delivery problem.
        await BasicAckAsync(eventArgs, channel, stoppingToken);
        _logger.LogInformation("Acknowledgement sent. MessageId: {MessageId}", message.MessageId);
    }

    private async Task DeadLetterInvalidAsync(BasicDeliverEventArgs eventArgs, IChannel channel, CancellationToken stoppingToken)
    {
        MessageMetrics.Failed.Inc();
        await ForwardAndAckAsync(_settings.DeadLetterQueueName, eventArgs, channel,
            eventArgs.BasicProperties.Headers, stoppingToken);
        MessageMetrics.DeadLetter.Inc();
        _logger.LogWarning("Invalid payload forwarded to dead-letter queue. DeliveryTag: {DeliveryTag}", eventArgs.DeliveryTag);
    }

    private async Task ProcessMessage(ImageCreatedMessage message, CancellationToken stoppingToken)
    {
        using Activity? activity = TracingConstants.StartMessagingActivity<ImageProcessingWorker>(nameof(ProcessMessage));
        activity?.SetTag("message.id", message.MessageId);
        activity?.SetTag("image.id", message.ImageId);
        activity?.SetTag("message.type", nameof(ImageCreatedMessage));

        _logger.LogInformation( "Activity created: {ActivityCreated}, TraceId: {TraceId}, SpanId: {SpanId}",
            activity != null, activity?.TraceId, activity?.SpanId);

        using var scope = _scopeFactory.CreateScope();
        var processService = scope.ServiceProvider.GetRequiredService<ProcessWorkerService>();

        var result = await processService.ProcessAsync(message, stoppingToken);

        if(result == ProcessWorkerResult.Duplicate)
        {
            return;
        }        

        _logger.LogInformation( "{messager} Message Status: {Status}, MessageId: {MessageId}", 
            nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.Processed), message.MessageId);
        MessageMetrics.Processed.Inc();

    }

    private async Task ProcessMessageException(BasicDeliverEventArgs eventArgs, IChannel channel, ImageCreatedMessage? message, Exception ex, CancellationToken stoppingToken)
    {
        using Activity? activity = TracingConstants.StartMessagingActivity<ImageProcessingWorker>(nameof(ProcessMessageException));
        activity?.SetTag("message.type", nameof(ImageCreatedMessage));
        activity?.SetTag("DeliveryTag", eventArgs.DeliveryTag);

        _logger.LogError(ex, "{messager} Message Status: {status}, DeliveryTag: {DeliveryTag}", nameof(ImageCreatedMessage),
            nameof(ProcessMessageStatus.Failed), eventArgs.DeliveryTag);
        MessageMetrics.Failed.Inc();

        var retryCount = GetRetryCount(eventArgs);

        var headers = eventArgs.BasicProperties.Headers == null
            ? new Dictionary<string, object?>() : new Dictionary<string, object?>(eventArgs.BasicProperties.Headers);

        if (retryCount >= _settings.MaxRetryCount)
        {
            await ForwardAndAckAsync(_settings.DeadLetterQueueName, eventArgs, channel, headers, stoppingToken);
            _logger.LogWarning("Delivery forwarded to dead-letter queue. MessageId: {MessageId}", message?.MessageId);
            MessageMetrics.DeadLetter.Inc();
            return;
        }

        headers[_settings.RetryHeader] = retryCount + 1;
        await ForwardAndAckAsync(_settings.RetryQueueName, eventArgs, channel, headers, stoppingToken);

        return; 
    }

    private async Task BasicAckAsync(BasicDeliverEventArgs eventArgs, IChannel channel, CancellationToken stoppingToken)
    {
        await channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        MessageMetrics.Acknowledged.Inc();
    }

    private async Task ForwardAndAckAsync(string destinationQueue, BasicDeliverEventArgs eventArgs, IChannel consumerChannel,
        IDictionary<string, object?>? headers, CancellationToken stoppingToken)
    {
        await _publisher.ForwardAsync(destinationQueue, eventArgs.Body, headers, stoppingToken);

        await BasicAckAsync(eventArgs, consumerChannel, stoppingToken);
    }
}
