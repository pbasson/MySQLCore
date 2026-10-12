namespace MySQLCore.Worker.BackgroundServices;

public abstract class BaseWorker<TMessage> : BackgroundService where TMessage: IMessage 
{
    public readonly ILogger<BaseWorker<TMessage>> _logger;

    public BaseWorker(ILogger<BaseWorker<TMessage>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Deserialize Message to Message object 
    /// </summary>
    protected static TMessage? DeserializeMessage(ReadOnlyMemory<byte> body)
    {
        return JsonSerializer.Deserialize<TMessage>(body.Span);
    }
}
