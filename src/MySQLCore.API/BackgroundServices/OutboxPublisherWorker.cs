namespace MySQLCore.API.BackgroundServices;

public class OutboxPublisherWorker : BackgroundService
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
            using var scope = _scopeFactory.CreateScope();

            var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxMessagerRepo>();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

            var messages = await outboxRepo.GetPendingAsync(10);

            foreach (var outbox in messages)
            {
                try
                {
                    var message = JsonSerializer.Deserialize<ImageCreatedMessage>(outbox.Payload);

                    if (message == null)
                    {
                        await outboxRepo.MarkFailedAsync(outbox.Id, "Invalid payload");
                        continue;
                    }

                    await publisher.PublishAsync(MessagerConstants.IMAGE_QUEUE, message);

                    await outboxRepo.MarkPublishedAsync(outbox.Id);

                    _logger.LogInformation( "Outbox message published. MessageId: {MessageId}", outbox.MessageId);
                }
                catch (Exception ex)
                {
                    await outboxRepo.MarkFailedAsync(outbox.Id, ex.Message);
                    _logger.LogError( ex, "Failed publishing outbox message. MessageId: {MessageId}", outbox.MessageId);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}