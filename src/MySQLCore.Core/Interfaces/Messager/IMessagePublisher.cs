namespace MySQLCore.Core.Interfaces.Messager;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(string queueName, TMessage message) where TMessage : IMessage;
}