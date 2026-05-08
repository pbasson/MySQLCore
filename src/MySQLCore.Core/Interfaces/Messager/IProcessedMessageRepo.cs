namespace MySQLCore.Core.Interfaces.Messager;

public interface IProcessedMessageRepo
{
    Task AddAsync(ProcessedMessage message);
    Task<bool> ExistsAsync(Guid messageId);
}