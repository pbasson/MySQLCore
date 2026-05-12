using Microsoft.Extensions.Options;

namespace MySQLCore.API.BackgroundServices;

public class BaseWorker<T> : BackgroundService where T: IMessage 
{
    public readonly ILogger<BaseWorker<T>> _logger;
    public readonly RabbitMQSettings _settings;

    public BaseWorker(ILogger<BaseWorker<T>> logger, IOptions<RabbitMQSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IChannel> CreateConnection(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = MessagerConstants.RabbitMQService(), UserName = _settings.UserName, Password = _settings.Password };
        var connection = await CreateConnectionWithRetryAsync(factory, stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(queue: MessagerConstants.IMAGE_QUEUE, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        return channel;
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
                _logger.LogWarning(ex, "RabbitMQ not ready. Retrying in {time} seconds...", time);
                await Task.Delay(TimeSpan.FromSeconds(time), stoppingToken);
            }
        }

        throw new OperationCanceledException();
    }

    public int GetRetryCount(BasicDeliverEventArgs eventArgs)
    {
        if (eventArgs.BasicProperties?.Headers == null)
        {
            return 0;
        }

        if (!eventArgs.BasicProperties.Headers.TryGetValue(_settings.RetryHeader, out var value))
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


    public T? DeserializeMessage(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);

        var message = JsonSerializer.Deserialize<T>(json);

        if (message == null)
        {
            _logger.LogWarning(
                "Message Status: {Status} - Payload: {Payload}", nameof(ProcessMessageStatus.Failed), json);
        }

        return message;
    }
}
