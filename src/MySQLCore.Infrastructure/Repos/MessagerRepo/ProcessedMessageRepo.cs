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

        int take = 100;
        var result = _dBContext.ProcessedMessage
            // .Where(x => x.Status == OutboxMessageStatus.Published )
            .OrderByDescending(x => x.ProcessedAt).Take(take).ToListAsync();
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

    public async Task<MessageProcessResult> ProcessImageCreatedAsync(ImageCreatedMessage message, CancellationToken cancellationToken)
    {
        if (message.MessageId == Guid.Empty)
            throw new ArgumentException("MessageId must not be empty.", nameof(message));

        var strategy = _dBContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dBContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Insert or lock the existing row without overwriting its completion state.
                await _dBContext.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO ProcessedMessage
                        (MessageId, MessageType, EntityName, EntityId, Status, ProcessedAt)
                    VALUES ({message.MessageId}, {nameof(ImageCreatedMessage)}, {"ImageTransaction"},
                        {message.ImageId}, {(int)ProcessMessageStatus.Processing}, {DateTime.UtcNow})
                    ON DUPLICATE KEY UPDATE Id = Id
                    """, cancellationToken);

                var records = await _dBContext.ProcessedMessage.FromSqlInterpolated($"""
                    SELECT * FROM ProcessedMessage WHERE MessageId = {message.MessageId} FOR UPDATE
                    """).AsNoTracking().ToListAsync(cancellationToken);
                var existing = records.Single();

                if (existing.EntityId != message.ImageId || existing.MessageType != nameof(ImageCreatedMessage))
                    throw new InvalidOperationException("MessageId is already associated with a different event.");

                if (existing.Status is ProcessMessageStatus.Processed or
                    ProcessMessageStatus.Acknowledged or ProcessMessageStatus.Completed)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return MessageProcessResult.Duplicate;
                }

                // The current handler only records completion. Future database work belongs here.
                await _dBContext.ProcessedMessage.Where(x => x.MessageId == message.MessageId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Status, ProcessMessageStatus.Processed)
                        .SetProperty(x => x.ProcessedAt, DateTime.UtcNow), cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return MessageProcessResult.Completed;
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        });
    }
}
