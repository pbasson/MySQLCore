using MySQLCore.Core.Messager;
using MySQLCore.Infrastructure.Messager;

namespace MySQLCore.API.BackgroundServices;

public class ImageProcessingWorker : BackgroundService
{
    private readonly ILogger<ImageProcessingWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public ImageProcessingWorker(ILogger<ImageProcessingWorker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var factory = new ConnectionFactory { HostName = MessagerConstants.RabbitMQService() };
        var factory = new ConnectionFactory { HostName = MessagerConstants.RabbitMQService(), UserName = "guest", Password = "password001" };

        var connection = await CreateConnectionWithRetryAsync(factory, stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync( queue: MessagerConstants.IMAGE_QUEUE, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<ImageCreatedMessage>(json);

            if (message != null)
            {
                _logger.LogInformation( "Received image message. MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}",
                    message.MessageId, message.ImageId, message.FileName);

                using var scope = _scopeFactory.CreateScope();

                var processService = scope.ServiceProvider.GetRequiredService<ProcessMessagePublisher>();
                await processService.ProcessAsync(message);

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
                Console.WriteLine("Started Shit");
                return await factory.CreateConnectionAsync(stoppingToken);
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
}