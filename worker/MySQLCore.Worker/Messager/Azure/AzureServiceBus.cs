namespace MySQLCore.Worker.Messager.Azure;

public class AzureServiceBus : IMessageBus
{
    public Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        throw new NotImplementedException();
    }
}