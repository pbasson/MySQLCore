using Microsoft.Extensions.Options;

namespace MySQLCore.API.BackgroundServices;

public class BaseWorker<TMessage> : BackgroundService where TMessage: IMessage 
{
    public readonly ILogger<BaseWorker<TMessage>> _logger;
    public readonly RabbitMQSettings _settings;
    public readonly RabbitMQConnectionService _connectionService;

    public BaseWorker(ILogger<BaseWorker<TMessage>> logger, IOptions<RabbitMQSettings> options, RabbitMQConnectionService connectionService)
    {
        _logger = logger;
        _settings = options.Value;
        _connectionService = connectionService;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
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

    /// <summary>
    /// Deserialize Message to Message object 
    /// </summary>
    public TMessage? DeserializeMessage(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);

        var message = JsonSerializer.Deserialize<TMessage>(json);

        if (message == null)
        {
            _logger.LogWarning( "Message Status: {Status} - Payload: {Payload}", nameof(ProcessMessageStatus.Failed), json);
        }

        return message;
    }


}
