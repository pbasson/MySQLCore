namespace MySQLCore.Worker.Interfaces.Worker;

public interface IRabbitMQBus : IMessageBus
{
    Task ForwardAsync(string queueName, ReadOnlyMemory<byte> body, IDictionary<string, object?>? headers, CancellationToken cancellationToken = default);
}