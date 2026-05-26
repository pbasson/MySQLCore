namespace MySQLCore.Core.Messager;

public class ProcessedMessageService : IProcessedMessageService
{
    private readonly IProcessedMessageRepo _processedMessageRepo;

    public ProcessedMessageService(IProcessedMessageRepo processedMessageRepo)
    {
        _processedMessageRepo = processedMessageRepo;
    }

    public Task<ProcessedMessage> GetMessage(Guid messageId)
    {
        return _processedMessageRepo.GetMessageById(messageId);
    }

    public Task<List<ProcessedMessage>> GetLatestProcessedMessages()
    {
        return _processedMessageRepo.GetLatestProcessedMessages();
    }
}