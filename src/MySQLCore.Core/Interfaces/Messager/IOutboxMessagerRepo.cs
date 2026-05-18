namespace MySQLCore.Core.Interfaces.Messager;

public interface IOutboxMessagerRepo
{
    Task AddAsync(OutboxMessage message);
    Task<bool> UpdateAsync(long outboxMessageId, OutboxMessageStatus status);
    Task<bool> ExistsAsync(Guid messageId);
}