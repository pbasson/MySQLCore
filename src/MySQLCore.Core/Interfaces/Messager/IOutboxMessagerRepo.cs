namespace MySQLCore.Core.Interfaces.Messager;

public interface IOutboxMessagerRepo
{
    Task<List<OutboxMessage>> GetPendingAsync(int take);
    Task<bool> AddAsync(OutboxMessage message);
    Task MarkPublishedAsync(long id);
    Task MarkFailedAsync(long id, string errorMessage);
    Task IncrementRetryAsync(long id, string errorMessage);

    Task<OutboxMessage> GetMessageById(long id);
    Task<List<OutboxMessage>> GetLatestPublishedMessages();
}