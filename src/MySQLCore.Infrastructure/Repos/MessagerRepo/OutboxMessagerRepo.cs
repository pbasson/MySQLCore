namespace MySQLCore.Infrastructure.Repos.MessagerRepo;

public sealed class OutboxMessagerRepo : BaseRepo<IBaseRepo>, IOutboxMessagerRepo
{
    public OutboxMessagerRepo(MySQLCoreDBContext dBContext, ILogger<IOutboxMessagerRepo> logger) : base(dBContext, logger) { }

    public async Task<List<OutboxMessage>> GetPendingAsync(int take)
    {
        return await _dBContext.OutboxMessage
            .Where(x => x.Status == OutboxMessageStatus.Pending || x.Status == OutboxMessageStatus.Failed)
            .OrderBy(x => x.CreatedAt).Take(take).ToListAsync();
    }

    public async Task<bool> AddAsync(OutboxMessage message)
    {
        _dBContext.OutboxMessage.Add(message);
        return await _dBContext.SaveChangesAsync() > 0;
    }

    public async Task MarkPublishedAsync(long id)
    {
        var message = await _dBContext.OutboxMessage.FindAsync(id);
        if (message == null)
        {
            _logger.LogWarning($"OutboxMessage with id {id} not found for marking as published.");
            return;
        }

        DateTime dateTime = DateTime.UtcNow;
        message.Status = OutboxMessageStatus.Published;
        message.PublishedAt = dateTime;
        message.LastAttemptAt = dateTime;
        message.ErrorMessage = null;

        await _dBContext.SaveChangesAsync();
    }

    public async Task MarkFailedAsync(long id, string errorMessage)
    {
        var message = await _dBContext.OutboxMessage.FindAsync(id);
        if (message == null)
        {
            _logger.LogWarning($"OutboxMessage with id {id} not found for marking as published.");
            return;
        }

        message.Status = OutboxMessageStatus.Failed;
        message.RetryCount++;
        message.LastAttemptAt = DateTime.UtcNow;
        message.ErrorMessage = errorMessage;

        await _dBContext.SaveChangesAsync();
    }

    public async Task IncrementRetryAsync(long id, string errorMessage)
    {
        var message = await _dBContext.OutboxMessage.FindAsync(id);
        if (message == null) return;

        message.RetryCount++;
        message.LastAttemptAt = DateTime.UtcNow;
        message.ErrorMessage = errorMessage;

        await _dBContext.SaveChangesAsync();
    }

    public async Task<OutboxMessage> GetMessageById(long id)
    {
        var result = await _dBContext.OutboxMessage.FindAsync(id);
        return result ?? throw new Exception($"OutboxMessage with id {id} not found.");
    }

    public Task<List<OutboxMessage>> GetLatestPublishedMessages()
    {
        return _dBContext.OutboxMessage.Where(x => x.Status == OutboxMessageStatus.Published )
            .OrderByDescending(x => x.PublishedAt).Take(1000).ToListAsync();
    }
}