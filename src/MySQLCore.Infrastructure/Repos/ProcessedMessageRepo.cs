namespace MySQLCore.Infrastructure.Repos;

public class ProcessedMessageRepo : BaseRepo, IProcessedMessageRepo
{
    public ProcessedMessageRepo(MySQLCoreDBContext dbContext) : base(dbContext) { }
    public async Task AddAsync(Guid messageId)
    {
        _dBContext.ProcessedMessage.Add(new (messageId ) );
        await SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid messageId)
    {
        return await _dBContext.ProcessedMessage.AnyAsync(x => x.MessageId == messageId);
    }
}