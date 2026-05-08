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
            _logger.LogInformation("Duplicate message ignored: {MessageId}", message.MessageId);
            return;
        }

        _logger.LogInformation( "Processing image. MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}",
             message.MessageId, message.ImageId, message.FileName);

        // actual processing here

        await _repo.AddAsync(new ProcessedMessageTransfer().GetTransfer(message.MessageId, nameof(ImageCreatedMessage), nameof(ImageTransaction), message.ImageId));
    }
}