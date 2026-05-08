using Microsoft.Extensions.Options;

namespace MySQLCore.API.BackgroundServices;

public class ImageProcessingWorker : BackgroundService
{
    private readonly ILogger<ImageProcessingWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMQSettings _settings;
    private const string RetryHeader = "x-retry-count";
    
    public ImageProcessingWorker(ILogger<ImageProcessingWorker> logger, IServiceScopeFactory scopeFactory, IOptions<RabbitMQSettings> options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var factory = new ConnectionFactory { HostName = MessagerConstants.RabbitMQService() };
        var factory = new ConnectionFactory { HostName = MessagerConstants.RabbitMQService(), UserName = _settings.UserName, Password = _settings.Password };
        var connection = await CreateConnectionWithRetryAsync(factory, stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync( queue: MessagerConstants.IMAGE_QUEUE, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            ImageCreatedMessage? message = null;

            try
            {
                var body = eventArgs.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                message = JsonSerializer.Deserialize<ImageCreatedMessage>(json);

                if (message == null)
                {   
                    _logger.LogWarning("Invalid image message received. Payload: {Payload}", json);
                    await channel.BasicAckAsync( deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);

                    return;
                }

                _logger.LogInformation( "Received image message. MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}",
                    message.MessageId, message.ImageId, message.FileName);

                using var scope = _scopeFactory.CreateScope();

                var processService = scope.ServiceProvider.GetRequiredService<ProcessMessageService>();
                await processService.ProcessAsync(message);

                await channel.BasicAckAsync( deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);

                _logger.LogInformation( "Message acknowledged. MessageId: {MessageId}", message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError( ex, "Message processing failed. MessageId: {MessageId}, DeliveryTag: {DeliveryTag}", 
                    message?.MessageId, eventArgs.DeliveryTag);

                var retryCount = GetRetryCount(eventArgs);

                if (retryCount >= _settings.MaxRetryCount)
                {
                    _logger.LogError("Message moved to DLQ after {RetryCount} retries", retryCount);

                    await channel.BasicPublishAsync( exchange: string.Empty, routingKey: _settings.DeadLetterQueueName, body: eventArgs.Body, cancellationToken: stoppingToken);
                    await channel.BasicAckAsync( deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);

                    return;
                }

                var properties = new BasicProperties { Headers = new Dictionary<string, object?> { [RetryHeader] = retryCount + 1 }, Persistent = true };

                await channel.BasicPublishAsync( exchange: string.Empty, routingKey: MessagerConstants.IMAGE_QUEUE,
                    mandatory: false, basicProperties: properties, body: eventArgs.Body, cancellationToken: stoppingToken);

                await channel.BasicAckAsync( deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync( queue: MessagerConstants.IMAGE_QUEUE, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }

    private async Task<IConnection> CreateConnectionWithRetryAsync( ConnectionFactory factory, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var getConnection = await factory.CreateConnectionAsync(stoppingToken);
                _logger.LogInformation( "RabbitMQ Connection Established");
                return getConnection; 
            }
            catch (Exception ex)
            {
                int time = 30;
                _logger.LogWarning(ex, $"RabbitMQ not ready. Retrying in {time} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(time), stoppingToken);
            }
        }

        throw new OperationCanceledException();
    }

    private static int GetRetryCount(BasicDeliverEventArgs eventArgs)
    {
        if (eventArgs.BasicProperties?.Headers == null)
        {
            return 0;
        }

        if (!eventArgs.BasicProperties.Headers.TryGetValue(RetryHeader, out var value))
        {
            return 0;
        }

        return value switch
        {
            byte[] bytes when int.TryParse(Encoding.UTF8.GetString(bytes), out var result) => result,
            int number => number,
            long number => (int)number,
            _ => 0
        };
    }
}
