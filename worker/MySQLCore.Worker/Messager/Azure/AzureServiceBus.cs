namespace MySQLCore.Worker.Messager.Azure;

public class AzureServiceBus : IMessageBus
{
    public Task ForwardAsync(string queueName, ReadOnlyMemory<byte> body, IDictionary<string, object?>? headers, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        throw new NotImplementedException();
    }
}