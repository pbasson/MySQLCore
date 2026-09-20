namespace MySQLCore.Worker.Messager;

public sealed class ProcessWorkerService
{
    private readonly ILogger<ProcessWorkerService> _logger;
    private readonly IProcessedMessageRepo _repo;

    public ProcessWorkerService(ILogger<ProcessWorkerService> logger, IProcessedMessageRepo repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task<ProcessWorkerResult> ProcessAsync(ImageCreatedMessage message, CancellationToken cancellationToken = default)
    {
        using Activity? activity = TracingConstants.StartMessagingActivity<ProcessWorkerService>(nameof(ProcessAsync));
        activity?.SetTag("message.id", message.MessageId);
        activity?.SetTag("image.id", message.ImageId);
        activity?.SetTag("message.type", nameof(ImageCreatedMessage));

        _logger.LogInformation( "{messager} Message Status: {status}, MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}", 
            nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.Pending), message.MessageId, message.ImageId, message.FileName);
        
        var result = await _repo.ProcessImageCreatedAsync(message, cancellationToken);
        if (result == MessageProcessResult.Duplicate)
        {
            _logger.LogInformation("{messager} Message Status: {status}, MessageId: {MessageId}", nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.IgnoredDuplicate), message.MessageId);
            MessageMetrics.Duplicate.Inc();
            return ProcessWorkerResult.Duplicate;
        }
      
        _logger.LogInformation( "{messager} Message Status: {Status}, MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}", nameof(ImageCreatedMessage),
            nameof(ProcessMessageStatus.Processed), message.MessageId, message.ImageId, message.FileName);
        return ProcessWorkerResult.Completed;
    }

    public async Task<bool> UpdateMessageStatusAsync(Guid messageId, ProcessMessageStatus status)
    {
        var result = await _repo.UpdateAsync(messageId, status);
        return result;
    }
}
