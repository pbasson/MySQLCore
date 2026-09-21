namespace MySQLCore.Worker.BackgroundServices.Outbox;

public sealed class OutboxPublisherWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherWorker> _logger;

    public OutboxPublisherWorker( IServiceScopeFactory scopeFactory,ILogger<OutboxPublisherWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                int messageCount = 10;
                using var scope = _scopeFactory.CreateScope();

                var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxMessagerRepo>();
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

                var messages = await outboxRepo.GetPendingAsync(messageCount);

                foreach (var outbox in messages)
                {
                    try
                    {
                        var message = JsonSerializer.Deserialize<ImageCreatedMessage>(outbox.Payload);

                        if (message == null || message.MessageId == Guid.Empty || message.MessageId != outbox.MessageId)
                        {
                            _logger.LogWarning("Invalid outbox payload. MessageId: {MessageId}", outbox.MessageId);
                            await outboxRepo.MarkDeadLetterAsync(outbox.Id, "Invalid payload: missing or mismatched MessageId.");
                            continue;
                        }

                        await publisher.PublishAsync(MessagerConstants.IMAGE_QUEUE, message, stoppingToken);

                        await outboxRepo.MarkPublishedAsync(outbox.Id);

                        _logger.LogInformation( "Outbox message published. MessageId: {MessageId}", outbox.MessageId);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Invalid outbox payload. MessageId: {MessageId}", outbox.MessageId);

                        await outboxRepo.MarkDeadLetterAsync(outbox.Id, ex.Message);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError( ex, "Failed publishing outbox message. MessageId: {MessageId}", outbox.MessageId);
                        await outboxRepo.MarkFailedAsync(outbox.Id, ex.Message);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox polling cycle failed. Retrying in 5 seconds.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
        }
    }
}
