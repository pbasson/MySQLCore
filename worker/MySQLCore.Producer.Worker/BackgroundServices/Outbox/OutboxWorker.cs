using MySQL.Core.Worker.Interfaces.Worker;

namespace MySQLCore.Producer.Worker.BackgroundServices.Outbox;

public sealed class OutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxWorker> _logger;
    private readonly string _queueName;

    public OutboxWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxWorker> logger,
        IConfiguration configuration, IOptions<AzureServiceBusSetting> azureOptions)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        // Use the same Azure queue as the consumer; RabbitMQ retains its existing queue.
        _queueName = configuration["Messaging:Provider"] switch
        {
            "AzureServiceBus" => azureOptions.Value.QueueName,
            "RabbitMQ" => MessagerConstants.IMAGE_QUEUE,
            _ => throw new InvalidOperationException("Messaging:Provider is Required.")
        };
        ArgumentException.ThrowIfNullOrWhiteSpace(_queueName);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                int messageCount = 10;
                using var scope = _scopeFactory.CreateScope();

                var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxMessagerRepo>();
                var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                var messages = await outboxRepo.GetPendingAsync(messageCount);

                foreach (var outbox in messages)
                {
                    try
                    {
                        var message = JsonSerializer.Deserialize<ImageGalleryMessage>(outbox.Payload);

                        if (message == null || message.MessageId == Guid.Empty || message.MessageId != outbox.MessageId)
                        {
                            _logger.LogWarning("Invalid outbox payload. MessageId: {MessageId}", outbox.MessageId);
                            await outboxRepo.MarkDeadLetterAsync(outbox.Id, "Invalid payload: missing or mismatched MessageId.");
                            continue;
                        }

                        await messageBus.PublishAsync(_queueName, message, cancellationToken);

                        await outboxRepo.MarkPublishedAsync(outbox.Id);

                        _logger.LogInformation( "Outbox message published. MessageId: {MessageId}", outbox.MessageId);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Invalid outbox payload. MessageId: {MessageId}", outbox.MessageId);

                        await outboxRepo.MarkDeadLetterAsync(outbox.Id, ex.Message);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
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
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox polling cycle failed. Retrying in 5 seconds.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
    }
}
