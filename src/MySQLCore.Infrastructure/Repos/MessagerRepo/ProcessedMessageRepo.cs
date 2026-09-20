namespace MySQLCore.Infrastructure.Repos.MessagerRepo;

public sealed class ProcessedMessageRepo : BaseRepo<IProcessedMessageRepo>, IProcessedMessageRepo
{
    public ProcessedMessageRepo(MySQLCoreDBContext dbContext, ILogger<IProcessedMessageRepo> logger) : base(dbContext, logger) { }
    
    public async Task AddAsync(ProcessedMessage message)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ProcessedMessageRepo>(nameof(AddAsync));

        _dBContext.ProcessedMessage.Add(message);
        await SaveChangesAsync(cancellationToken: CancellationToken.None);
    }

    public async Task<bool> ExistsAsync(Guid messageId)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ProcessedMessageRepo>(nameof(ExistsAsync));

        return await _dBContext.ProcessedMessage.AnyAsync(x => x.MessageId == messageId);
    }

    public async Task<ProcessedMessage> GetMessageById(Guid messageId)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ProcessedMessageRepo>(nameof(GetMessageById));
        activity?.SetTag("messageId", messageId);

        var result = await _dBContext.ProcessedMessage.FirstOrDefaultAsync(x => x.MessageId == messageId);
        return result ?? throw new Exception($"Message with MessageId {messageId} not found.");
    }

    public Task<List<ProcessedMessage>> GetLatestProcessedMessages()
    {
        using Activity? activity = TracingConstants.StartApiActivity<ProcessedMessageRepo>(nameof(GetLatestProcessedMessages));

        var result = _dBContext.ProcessedMessage
            // .Where(x => x.Status == OutboxMessageStatus.Published )
            .OrderByDescending(x => x.ProcessedAt).Take(1000).ToListAsync();
        return result;
    }

    public async Task<bool> UpdateAsync(Guid messageId, ProcessMessageStatus status)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ProcessedMessageRepo>(nameof(UpdateAsync));
        activity?.SetTag("messageId", messageId);
        activity?.SetTag("status", status.ToString());

        var message = await GetMessageById(messageId);
        message.Status = status;
        message.ProcessedAt = DateTime.UtcNow;

        _dBContext.ProcessedMessage.Update(message);
        var test = CancellationToken.None;
        return await SaveChangesAsync(cancellationToken: test);
    }
}
