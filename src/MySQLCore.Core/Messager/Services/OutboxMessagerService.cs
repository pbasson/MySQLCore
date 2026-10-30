namespace MySQLCore.Core.Messager.Services;

public class OutboxMessagerService : IOutboxMessagerService
{
    private readonly IOutboxMessageRepo _outboxRepo;

    public OutboxMessagerService(IOutboxMessageRepo outboxRepo)
    {
        _outboxRepo = outboxRepo;
    }

    public Task<List<OutboxMessage>> GetPendingAsync(int take)
    {
        return _outboxRepo.GetPendingAsync(take);
    }

    public Task<OutboxMessage> GetOutboxMessage(long id)
    {
        return _outboxRepo.GetMessageById(id);
    }

    public Task<List<OutboxMessage>> GetLatestPublishedOutboxMessage()
    {
        return _outboxRepo.GetLatestPublishedMessages();
    }
}