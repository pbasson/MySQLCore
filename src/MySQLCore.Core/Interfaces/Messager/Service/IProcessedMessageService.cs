namespace MySQLCore.Core.Interfaces.Messager.Service;

public interface IProcessedMessageService
{
    Task<ProcessedMessage> GetMessage(Guid messageId);
    Task<List<ProcessedMessage>> GetLatestProcessedMessages();
}