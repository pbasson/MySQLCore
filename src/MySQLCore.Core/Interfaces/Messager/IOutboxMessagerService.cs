namespace MySQLCore.Core.Interfaces.Messager;

public interface IOutboxMessagerService
{
    Task<List<OutboxMessage>> GetPendingAsync(int take);
    Task<OutboxMessage> GetOutboxMessage(long id);
    Task<List<OutboxMessage>> GetLatestPublishedOutboxMessage();
}