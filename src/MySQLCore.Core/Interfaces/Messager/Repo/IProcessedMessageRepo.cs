namespace MySQLCore.Core.Interfaces.Messager.Repo;

public interface IProcessedMessageRepo
{
    Task AddAsync(ProcessedMessage message);
    Task<bool> ExistsAsync(Guid messageId);
    Task<bool> UpdateAsync(Guid messageId, ProcessMessageStatus status);

    Task<ProcessedMessage> GetMessageById(Guid messageId);
    Task<List<ProcessedMessage>> GetLatestProcessedMessages();
}