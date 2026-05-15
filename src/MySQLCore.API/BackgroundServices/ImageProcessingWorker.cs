namespace MySQLCore.API.BackgroundServices;

public class ImageProcessingWorker : BaseWorker<ImageCreatedMessage>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public ImageProcessingWorker(ILogger<ImageProcessingWorker> logger, IServiceScopeFactory scopeFactory, IOptions<RabbitMQSettings> options, RabbitMQConnectionService connectionService)
     : base(logger, options, connectionService)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IChannel channel = await _connectionService.CreateConnection(stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        _logger.LogInformation("{messager} Message Status: {status}", nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.Received));
        MessageMetrics.Received.Inc();

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            ImageCreatedMessage? message = null;

            try
            {
                message = DeserializeMessage(eventArgs);

                if (message == null)
                {
                    await BasicAckAsync(eventArgs, channel, stoppingToken);
                    return;
                }

                await ProcessMessage(message, eventArgs, channel, stoppingToken);
            }
            catch (Exception ex)
            {
                await ProcessMessageException(eventArgs, channel, message, ex, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(queue: MessagerConstants.IMAGE_QUEUE, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }

    private async Task ProcessMessage( ImageCreatedMessage message, BasicDeliverEventArgs eventArgs, IChannel channel, CancellationToken stoppingToken)
    {
        _logger.LogInformation( "{messager} Message Status: {Status}, MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}", nameof(ImageCreatedMessage),
            nameof(ProcessMessageStatus.Processing), message.MessageId, message.ImageId, message.FileName);
        MessageMetrics.Processing.Inc();

        using var scope = _scopeFactory.CreateScope();

        var processService = scope.ServiceProvider.GetRequiredService<ProcessMessageService>();

        await processService.ProcessAsync(message);
        _logger.LogInformation( "{messager} Message Status: {Status}, MessageId: {MessageId}", nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.Processed), message.MessageId);
        MessageMetrics.Processed.Inc();

        await BasicAckAsync(eventArgs, channel, stoppingToken);
        _logger.LogInformation( "{messager} Message Status: {Status}, MessageId: {MessageId}", nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.Acknowledged), message.MessageId);
        MessageMetrics.Acknowledged.Inc();
    }

    private async Task ProcessMessageException(BasicDeliverEventArgs eventArgs, IChannel channel, ImageCreatedMessage? message, Exception ex, CancellationToken stoppingToken)
    {
        _logger.LogError(ex, "{messager} Message Status: {status}, MessageId: {MessageId}, DeliveryTag: {DeliveryTag}", nameof(ImageCreatedMessage),
            nameof(ProcessMessageStatus.Failed), message?.MessageId, eventArgs.DeliveryTag);
        MessageMetrics.Failed.Inc();

        var retryCount = GetRetryCount(eventArgs);
        if (retryCount >= _settings.MaxRetryCount)
        {
            _logger.LogError("{messager} Message Status: {status} - moved to DLQ after {RetryCount} retries", nameof(ImageCreatedMessage), ProcessMessageStatus.DeadLetter, retryCount);

            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: _settings.DeadLetterQueueName, body: eventArgs.Body, cancellationToken: stoppingToken);
            await BasicAckAsync(eventArgs, channel, stoppingToken);
            MessageMetrics.DeadLetter.Inc();
            return;
        }

        var properties = new BasicProperties { Headers = new Dictionary<string, object?> { [_settings.RetryHeader] = retryCount + 1 }, Persistent = true };

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: MessagerConstants.IMAGE_QUEUE,
            mandatory: false, basicProperties: properties, body: eventArgs.Body, cancellationToken: stoppingToken);

        await BasicAckAsync(eventArgs, channel, stoppingToken);
        return; 
    }

    private async Task BasicAckAsync(BasicDeliverEventArgs eventArgs, IChannel channel, CancellationToken stoppingToken)
    {
        await channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
    }
}
