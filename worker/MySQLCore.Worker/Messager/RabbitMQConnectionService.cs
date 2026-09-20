namespace MySQLCore.Worker.Messager;

public class RabbitMQConnectionService : IAsyncDisposable
{
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;
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
    public async Task<IChannel> CreateChannelAsync(CancellationToken stoppingToken, bool publisherConfirmations = false)
    {
        await _connectionLock.WaitAsync(stoppingToken);
        try
        {
            if (_connection == null)
            {
                var factory = new ConnectionFactory
                {
                    HostName = MessagerConstants.RabbitMQService(),
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    AutomaticRecoveryEnabled = true
                };
                _connection = await CreateConnectionWithRetryAsync(factory, stoppingToken);
            }

            // Keep the same connection so automatic recovery can restore the consumer too.
            var channel = await _connection.CreateChannelAsync(
                new CreateChannelOptions(publisherConfirmations, publisherConfirmations), stoppingToken);
            try
            {
                await channel.QueueDeclareAsync(queue: MessagerConstants.IMAGE_QUEUE, durable: true,
                    exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

                await channel.QueueDeclareAsync(queue: _settings.DeadLetterQueueName, durable: true,
                    exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

                var retryArguments = new Dictionary<string, object?>
                {
                    ["x-message-ttl"] = 30_000,
                    ["x-dead-letter-exchange"] = "",
                    ["x-dead-letter-routing-key"] = MessagerConstants.IMAGE_QUEUE
                };

                await channel.QueueDeclareAsync(queue: _settings.RetryQueueName, durable: true,
                    exclusive: false, autoDelete: false, arguments: retryArguments, cancellationToken: stoppingToken); 
               
                return channel;
            }
            catch
            {
                await channel.DisposeAsync();
                throw;
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        _connectionLock.Dispose();
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
    
