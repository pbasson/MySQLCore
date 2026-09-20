namespace MySQLCore.Core.Interfaces.Messager.Repo;

public interface IProcessedMessageRepo : IBaseRepo
{
    Task AddAsync(ProcessedMessage message);
    Task<bool> ExistsAsync(Guid messageId);
    Task<bool> UpdateAsync(Guid messageId, ProcessMessageStatus status);
    Task<MessageProcessResult> ProcessImageCreatedAsync(ImageCreatedMessage message, CancellationToken cancellationToken);
    Task<ProcessedMessage> GetMessageById(Guid messageId);
    Task<List<ProcessedMessage>> GetLatestProcessedMessages();
}