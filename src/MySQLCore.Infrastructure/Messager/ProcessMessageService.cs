namespace MySQLCore.Infrastructure.Messager;

public class ProcessMessageService
{
    private readonly ILogger<ProcessMessageService> _logger;
    private readonly IProcessedMessageRepo _repo;

    public ProcessMessageService(ILogger<ProcessMessageService> logger, IProcessedMessageRepo repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task ProcessAsync(ImageCreatedMessage message)
    {
        if (await _repo.ExistsAsync(message.MessageId))
        {
            _logger.LogInformation("{messager} Message Status: {status}, MessageId: {MessageId}", nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.IgnoredDuplicate), message.MessageId);
            return;
        }

        _logger.LogInformation( "{messager} Message Status: {status}, MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}", 
            nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.Pending), message.MessageId, message.ImageId, message.FileName);

        await _repo.AddAsync(new ProcessedMessageTransfer().GetTransfer(message.MessageId, nameof(ImageCreatedMessage), nameof(ImageTransaction), message.ImageId));
    }
}