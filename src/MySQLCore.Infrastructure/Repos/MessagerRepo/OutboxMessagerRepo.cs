using System.Linq.Expressions;

namespace MySQLCore.Infrastructure.Repos.MessagerRepo;

public sealed class OutboxMessagerRepo : BaseRepo<IBaseRepo>, IOutboxMessagerRepo
{
    public OutboxMessagerRepo(MySQLCoreDBContext dBContext, ILogger<IOutboxMessagerRepo> logger) : base(dBContext, logger) { }

    public async Task<List<OutboxMessage>> GetPendingAsync(int take)
    {
        var dateNow = DateTime.UtcNow;
        return await _dBContext.OutboxMessage.Where(Filter(dateNow)).OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id).Take(take).ToListAsync();

        static Expression<Func<OutboxMessage, bool>> Filter(DateTime now) => x => x.Status == OutboxMessageStatus.Pending 
            || (x.Status == OutboxMessageStatus.Failed && (x.NextAttemptAt == null || x.NextAttemptAt <= now));
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
            _logger.LogWarning("OutboxMessage with id {id} not found for marking as published.", id);
            return;
        }

        DateTime dateTime = DateTime.UtcNow;
        message.Status = OutboxMessageStatus.Published;
        message.PublishedAt = dateTime;
        message.LastAttemptAt = dateTime;
        message.ErrorMessage = null;
        message.NextAttemptAt = null;

        await _dBContext.SaveChangesAsync();
    }

    public async Task MarkFailedAsync(long id, string errorMessage)
    {
        var message = await _dBContext.OutboxMessage.FindAsync(id);
        if (message == null)
        {
            _logger.LogWarning("OutboxMessage with id {id} not found for marking as published.", id);
            return;
        }

        var now = DateTime.UtcNow;

        message.RetryCount++;
        message.LastAttemptAt = now;
        message.ErrorMessage = errorMessage;

        TimeSpan? delay = message.RetryCount switch
        {
            1 => TimeSpan.FromSeconds(5),
            2 => TimeSpan.FromSeconds(30),
            3 => TimeSpan.FromMinutes(2),
            4 => TimeSpan.FromMinutes(10),
            _ => null
        };

        message.Status = delay.HasValue ? OutboxMessageStatus.Failed : OutboxMessageStatus.DeadLetter;
        message.NextAttemptAt = delay.HasValue ? now.Add(delay.Value) : null;

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

    public async Task MarkDeadLetterAsync(long id, string errorMessage)
    {
        var message = await _dBContext.OutboxMessage.FindAsync(id);

        if (message == null)
        {
            _logger.LogWarning( "OutboxMessage with id {id} not found for marking as DeadLetter.", id);
            return;
        }

        message.Status = OutboxMessageStatus.DeadLetter;
        message.RetryCount++;
        message.LastAttemptAt = DateTime.UtcNow;
        message.NextAttemptAt = null;
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