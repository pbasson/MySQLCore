namespace MySQLCore.Worker.Messager.Azure;

public sealed class AzureServiceBus : IMessageBus, IAsyncDisposable
{
    private readonly ILogger<AzureServiceBus> _logger;
    private readonly ServiceBusClient _client;
    private readonly Dictionary<string, ServiceBusSender> _senders = new(StringComparer.Ordinal);
    private readonly object _senderLock = new();
    private bool _disposed;

    public AzureServiceBus(ILogger<AzureServiceBus> logger, ServiceBusClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);
        ArgumentNullException.ThrowIfNull(message);
        if (message.MessageId == Guid.Empty)
            throw new ArgumentException("A non-empty MessageId is required.", nameof(message));
        cancellationToken.ThrowIfCancellationRequested();

        using Activity? activity = TracingConstants.MessagingActivitySource.StartActivity(
            $"{nameof(AzureServiceBus)}.{nameof(PublishAsync)}", ActivityKind.Producer);
        activity?.SetTag("messaging.system", "servicebus");
        activity?.SetTag("messaging.destination.name", queueName);
        activity?.SetTag("messaging.message.id", message.MessageId.ToString());
        activity?.SetTag("message.type", typeof(TMessage).Name);

        try
        {
            byte[] body = JsonSerializer.SerializeToUtf8Bytes(message);
            var azureMessage = new ServiceBusMessage(body)
            {
                MessageId = message.MessageId.ToString(),
                ContentType = "application/json"
            };

            var sender = GetSender(queueName);
            // Only report success after Azure accepts the send. Failures reach OutboxWorker for retry.
            await sender.SendMessageAsync(azureMessage, cancellationToken);

            _logger.LogInformation("{QueueName} Message Status: {Status}, MessageId: {MessageId}",
                queueName, nameof(ProcessMessageStatus.Published), message.MessageId);
            MessageMetric.Published.Inc();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Azure message publishing failed");
            activity?.SetTag("error.type", ex.GetType().FullName);
            throw;
        }
    }

    private ServiceBusSender GetSender(string queueName)
    {
        lock (_senderLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_senders.TryGetValue(queueName, out var sender))
            {
                sender = _client.CreateSender(queueName);
                _senders.Add(queueName, sender);
            }
            return sender;
        }
    }

    public async ValueTask DisposeAsync()
    {
        ServiceBusSender[] senders;
        lock (_senderLock)
        {
            if (_disposed) return;
            _disposed = true;
            senders = _senders.Values.ToArray();
            _senders.Clear();
        }

        // Dispose owned senders; dependency injection owns the shared ServiceBusClient.
        await Task.WhenAll(senders.Select(sender => sender.DisposeAsync().AsTask()));
    }
}
