
namespace MySQLCore.Infrastructure.Repos.MessagerRepo;

public class ProcessedMessageRepo : BaseRepo, IProcessedMessageRepo
{
    public ProcessedMessageRepo(MySQLCoreDBContext dbContext) : base(dbContext) { }
    
    public async Task AddAsync(ProcessedMessage message)
    {
        _dBContext.ProcessedMessage.Add(message);
        await SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid messageId)
    {
        return await _dBContext.ProcessedMessage.AnyAsync(x => x.MessageId == messageId);
    }

    public async Task<ProcessedMessage> GetMessageById(Guid messageId)
    {
        var result = await _dBContext.ProcessedMessage.FirstOrDefaultAsync(x => x.MessageId == messageId);
        return result ?? throw new Exception($"Message with MessageId {messageId} not found.");
    }

    public Task<List<ProcessedMessage>> GetLatestProcessedMessages()
    {
        var result = _dBContext.ProcessedMessage
            // .Where(x => x.Status == OutboxMessageStatus.Published )
            .OrderByDescending(x => x.ProcessedAt).Take(1000).ToListAsync();
        return result;
    }
}