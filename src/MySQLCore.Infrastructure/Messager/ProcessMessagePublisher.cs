namespace MySQLCore.Infrastructure.Messager;

public class ProcessMessagePublisher
{
    private readonly ILogger<ProcessMessagePublisher> _logger;
    private readonly IProcessedMessageRepo _repo;

    public ProcessMessagePublisher(ILogger<ProcessMessagePublisher> logger, IProcessedMessageRepo repo)
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


        await _repo.AddAsync(message.MessageId);
    }
}