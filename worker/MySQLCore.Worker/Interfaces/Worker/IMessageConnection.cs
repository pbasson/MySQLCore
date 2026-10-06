namespace MySQLCore.Worker.Interfaces.Worker;

public interface IMessageConnection : IAsyncDisposable
{
    Task<T> CreateChannelAsync<T>(CancellationToken stoppingToken, bool publisherConfirmations = false) where T : IChannel;
}
