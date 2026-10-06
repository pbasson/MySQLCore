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

    public async Task<ProcessWorkerResult> ProcessAsync(ImageGalleryMessage message, CancellationToken cancellationToken = default)
    {
        using Activity? activity = TracingConstants.StartMessagingActivity<ProcessWorkerService>(nameof(ProcessAsync));
        activity?.SetTag("message.id", message.MessageId);
        activity?.SetTag("image.id", message.ImageId);
        activity?.SetTag("message.type", nameof(ImageGalleryMessage));

        try
        {
            _logger.LogInformation( "{messager} Message Status: {status}, MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}",
                nameof(ImageGalleryMessage), nameof(ProcessMessageStatus.Pending), message.MessageId, message.ImageId, message.FileName);

            var result = await _repo.ProcessImageCreatedAsync(message, cancellationToken);
            if (result == MessageProcessResult.Duplicate)
            {
                _logger.LogInformation("{messager} Message Status: {status}, MessageId: {MessageId}", nameof(ImageGalleryMessage), nameof(ProcessMessageStatus.IgnoredDuplicate), message.MessageId);
                MessageMetric.Duplicate.Inc();
                return ProcessWorkerResult.Duplicate;
            }

            _logger.LogInformation( "{messager} Message Status: {Status}, MessageId: {MessageId}, ImageId: {ImageId}, FileName: {FileName}", nameof(ImageGalleryMessage),
                nameof(ProcessMessageStatus.Processed), message.MessageId, message.ImageId, message.FileName);
            return ProcessWorkerResult.Completed;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Image processing failed");
            activity?.SetTag("error.type", ex.GetType().FullName);
            throw;
        }
    }

    public async Task<bool> UpdateMessageStatusAsync(Guid messageId, ProcessMessageStatus status)
    {
        var result = await _repo.UpdateAsync(messageId, status);
        return result;
    }
}
