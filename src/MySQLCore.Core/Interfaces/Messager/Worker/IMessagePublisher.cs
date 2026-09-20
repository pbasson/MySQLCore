namespace MySQLCore.Core.Interfaces.Messager.Worker;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage;
}
