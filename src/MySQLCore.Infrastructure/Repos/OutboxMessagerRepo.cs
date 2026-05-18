namespace MySQLCore.Infrastructure.Repos;

public class OutboxMessagerRepo : BaseRepo, IOutboxMessagerRepo
{

    public OutboxMessagerRepo(MySQLCoreDBContext dBContext) : base(dBContext) { }

    public async Task AddAsync(OutboxMessage message)
    {
        _dBContext.OutboxMessage.Add(message);
        await _dBContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(long outboxMessageId, OutboxMessageStatus status)
    {
        var message = await _dBContext.OutboxMessage.FindAsync(outboxMessageId);
        if (message != null)
        {
            message.Status = status;
            return await _dBContext.SaveChangesAsync() > 0;
        }
        return false;
    }

    public async Task<bool> ExistsAsync(Guid messageId)
    {
        return await _dBContext.OutboxMessage.AnyAsync(m => m.MessageId == messageId);
    }
}