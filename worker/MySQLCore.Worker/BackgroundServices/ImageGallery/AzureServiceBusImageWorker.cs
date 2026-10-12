namespace MySQLCore.Worker.BackgroundServices.ImageGallery;

public sealed class AzureServiceBusImageWorker : BaseWorker<ImageGalleryMessage>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ServiceBusClient _client;
    private readonly AzureServiceBusSetting _settings;

    public AzureServiceBusImageWorker(ILogger<AzureServiceBusImageWorker> logger, ServiceBusClient client,
        IOptions<AzureServiceBusSetting> options, IServiceScopeFactory scopeFactory): base(logger)
    {
        _client = client;
        _settings = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var processor = _client.CreateProcessor(_settings.QueueName, SetOptions() );
        processor.ProcessMessageAsync += async args =>
        {
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, args.CancellationToken);
            await HandleMessageAsync(args, cancellation.Token);
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Azure image consumer error. Source: {Source}, Queue: {Queue}",
                args.ErrorSource, args.EntityPath);
            return Task.CompletedTask;
        };

        try
        {
            await processor.StartProcessingAsync(cancellationToken);
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Stop receiving and drain callbacks before disposing the processor.
        }
        finally
        {
            await processor.StopProcessingAsync(CancellationToken.None);
        }

        static ServiceBusProcessorOptions SetOptions() => new()
        {
            AutoCompleteMessages = false,
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            MaxConcurrentCalls = 1,
            PrefetchCount = 0
        };
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args, CancellationToken cancellationToken)
    {
        using Activity? activity = TracingConstants.MessagingActivitySource.StartActivity(
            $"{nameof(AzureServiceBusImageWorker)}.{nameof(HandleMessageAsync)}", ActivityKind.Consumer);
        activity?.SetTag("messaging.system", "servicebus");
        activity?.SetTag("messaging.destination.name", _settings.QueueName);
        activity?.SetTag("message.type", nameof(ImageGalleryMessage));

        try
        {
            MessageMetric.Received.Inc();
            ImageGalleryMessage? message;
            try
            {
                message = DeserializeMessage(args.Message.Body.ToMemory());
            }
            catch (JsonException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Malformed image payload");
                _logger.LogWarning(ex, "Malformed Azure image message. MessageId: {MessageId}", args.Message.MessageId);
                await DeadLetterInvalidAsync(args, cancellationToken);
                return;
            }

            if (message == null || message.MessageId == Guid.Empty)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Missing message or MessageId");
                await DeadLetterInvalidAsync(args, cancellationToken);
                return;
            }

            activity?.SetTag("message.id", message.MessageId);
            activity?.SetTag("image.id", message.ImageId);
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ProcessWorkerService>();
                var result = await service.ProcessAsync(message, cancellationToken);
                if (result == ProcessWorkerResult.Completed)
                    MessageMetric.Processed.Inc();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Image processing failed");
                activity?.SetTag("error.type", ex.GetType().FullName);
                MessageMetric.Failed.Inc();
                _logger.LogError(ex, "Azure image processing failed. MessageId: {MessageId}, DeliveryCount: {DeliveryCount}",
                    message.MessageId, args.Message.DeliveryCount);
                // The Azure queue's MaxDeliveryCount limits retries; abandonment does not delay redelivery.
                await args.AbandonMessageAsync(args.Message, cancellationToken: cancellationToken);
                return;
            }

            // Keep settlement failures separate from processing failures. Redelivery uses existing deduplication.
            await args.CompleteMessageAsync(args.Message, cancellationToken);
            MessageMetric.Acknowledged.Inc();
            _logger.LogInformation("Azure message completed. MessageId: {MessageId}", message.MessageId);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Includes complete/abandon/dead-letter failures; let the processor report the exception.
            activity?.SetStatus(ActivityStatusCode.Error, "Message delivery failed");
            activity?.SetTag("error.type", ex.GetType().FullName);
            throw;
        }
    }

    private async Task DeadLetterInvalidAsync(ProcessMessageEventArgs args, CancellationToken cancellationToken)
    {
        MessageMetric.Failed.Inc();
        await args.DeadLetterMessageAsync(args.Message, "InvalidPayload",
            "Expected an image message with a non-empty MessageId.", cancellationToken);
        MessageMetric.DeadLetter.Inc();
        _logger.LogWarning("Invalid Azure image message dead-lettered. MessageId: {MessageId}", args.Message.MessageId);
    }
}
