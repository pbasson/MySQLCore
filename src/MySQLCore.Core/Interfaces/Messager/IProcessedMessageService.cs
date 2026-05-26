namespace MySQLCore.Core.Interfaces.Messager;

public interface IProcessedMessageService
{
    Task<ProcessedMessage> GetMessage(Guid messageId);
    Task<List<ProcessedMessage>> GetLatestProcessedMessages();
}