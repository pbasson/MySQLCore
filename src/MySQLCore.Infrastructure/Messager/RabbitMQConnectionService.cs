using Microsoft.Extensions.Options;

namespace MySQLCore.Infrastructure.Messager;

public class RabbitMQConnectionService
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQConnectionService> _logger;

    public RabbitMQConnectionService(IOptions<RabbitMQSettings> options, ILogger<RabbitMQConnectionService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Setup connection to RabbitMQ with Retry
    /// </summary>
    public async Task<IChannel> CreateConnection(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = MessagerConstants.RabbitMQService(), UserName = _settings.UserName, Password = _settings.Password };
        var connection = await CreateConnectionWithRetryAsync(factory, stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(queue: MessagerConstants.IMAGE_QUEUE, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        return channel;
    }

    /// <summary>
    /// 
    /// </summary>
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
                _logger.LogWarning(ex, "RabbitMQ not ready. Retrying in {time} seconds...", time);
                await Task.Delay(TimeSpan.FromSeconds(time), stoppingToken);
            }
        }

        throw new OperationCanceledException();
    }

    public byte[] SerializeMessage<TMessage>(TMessage message) where TMessage : IMessage
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        return body;
    }
 
}
    

