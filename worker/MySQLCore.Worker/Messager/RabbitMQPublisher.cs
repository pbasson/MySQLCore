namespace MySQLCore.Worker.Messager;

public class RabbitMQPublisher : IMessagePublisher
{
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly RabbitMQConnectionService _connectionService;
    public RabbitMQPublisher(ILogger<RabbitMQPublisher> logger, RabbitMQConnectionService connectionService)
    {
        _logger = logger;
        _connectionService = connectionService;
    }

    public async Task PublishAsync<TMessage>(string queueName, TMessage message) where TMessage : IMessage
    {
        var channel = await _connectionService.CreateConnection(CancellationToken.None);

        /// RabbitMQ only accepts Bytes hence payload must be serialized JSON to get bytes
        byte[] body = _connectionService.SerializeMessage(message);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
        _logger.LogInformation("{QueueName} Message Status: {status} ", queueName, nameof(ProcessMessageStatus.Published));
        MessageMetrics.Published.Inc();
    }
}
