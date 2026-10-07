namespace MySQLCore.Worker.Interfaces.Worker;

public interface IMessageBus
{
    Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage;
}
