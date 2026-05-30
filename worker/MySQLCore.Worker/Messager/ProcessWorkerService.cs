using MySQLCore.Worker.Enums;

namespace MySQLCore.Worker.Messager;

public class ProcessWorkerService
{
    private readonly ILogger<ProcessWorkerService> _logger;
    private readonly IProcessedMessageRepo _repo;

    public ProcessWorkerService(ILogger<ProcessWorkerService> logger, IProcessedMessageRepo repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task<ProcessWorkerResult> ProcessAsync(ImageCreatedMessage message)
    {
        using Activity? activity = TracingConstants.StartMessagingActivity<ProcessWorkerService>(nameof(ProcessAsync));
        activity?.SetTag("message.id", message.MessageId);
        activity?.SetTag("image.id", message.ImageId);
        activity?.SetTag("message.type", nameof(ImageCreatedMessage));

        _logger.LogInformation( "{messager} Message Status: {status}, MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}", 
            nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.Pending), message.MessageId, message.ImageId, message.FileName);
        
        if (await _repo.ExistsAsync(message.MessageId))
        {
            await _repo.UpdateAsync(message.MessageId, ProcessMessageStatus.IgnoredDuplicate);
            _logger.LogInformation("{messager} Message Status: {status}, MessageId: {MessageId}", nameof(ImageCreatedMessage), nameof(ProcessMessageStatus.IgnoredDuplicate), message.MessageId);
            MessageMetrics.Duplicate.Inc();
            return ProcessWorkerResult.Duplicate;
        }
      
        await _repo.AddAsync(new ProcessedMessageTransfer().GetTransfer(message.MessageId, nameof(ImageCreatedMessage), "ImageTransaction", message.ImageId));
        
        _logger.LogInformation( "{messager} Message Status: {Status}, MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}", nameof(ImageCreatedMessage),
            nameof(ProcessMessageStatus.Processing), message.MessageId, message.ImageId, message.FileName);
        MessageMetrics.Processing.Inc();
        return ProcessWorkerResult.Completed;
    }

    public async Task<bool> UpdateMessageStatusAsync(Guid messageId, ProcessMessageStatus status)
    {
        var result = await _repo.UpdateAsync(messageId, status);
        return result;
    }
}
