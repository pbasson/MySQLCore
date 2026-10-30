namespace MySQL.Core.Worker.Messager.RabbitMQ;

public sealed class RabbitMQBus : IRabbitMQBus, IAsyncDisposable
{
    private readonly ILogger<RabbitMQBus> _logger;
    private readonly IRabbitMQConnection _service;
    private IChannel? _channel;
    private readonly SemaphoreSlim _publishLock = new(1, 1);
    private int _disposeRequested;

    public RabbitMQBus(ILogger<RabbitMQBus> logger, IRabbitMQConnection service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);
        ArgumentNullException.ThrowIfNull(message);
        if (message.MessageId == Guid.Empty)
            throw new ArgumentException("A non-empty MessageId is required.", nameof(message));
        cancellationToken.ThrowIfCancellationRequested();
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposeRequested) != 0, this);

        using Activity? activity = TracingConstants.MessagingActivitySource.StartActivity(
            $"{nameof(RabbitMQBus)}.{nameof(PublishAsync)}", ActivityKind.Producer);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination.name", queueName);
        activity?.SetTag("messaging.message.id", message.MessageId.ToString());
        activity?.SetTag("message.type", typeof(TMessage).Name);

        try
        {
            byte[] body = JsonSerializer.SerializeToUtf8Bytes(message);
            await ForwardAsync(queueName, body, headers: null, cancellationToken: cancellationToken);

            // ForwardAsync awaits publisher confirmation before success is reported.
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
            activity?.SetStatus(ActivityStatusCode.Error, "RabbitMQ message publishing failed");
            activity?.SetTag("error.type", ex.GetType().FullName);
            throw;
        }
    }

    public async Task ForwardAsync(string queueName, ReadOnlyMemory<byte> body, IDictionary<string, object?>? headers, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposeRequested) != 0, this);

        await _publishLock.WaitAsync(cancellationToken);
        try
        {
            // Disposal may have started while this publish was waiting for the channel.
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposeRequested) != 0, this);
            if (_channel == null || !_channel.IsOpen)
            {
                if (_channel != null) await _channel.DisposeAsync();

                _channel = null;
                _channel = await _service.CreateChannelAsync(cancellationToken, publisherConfirmations: true);
            }

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Headers = headers == null ? null : new Dictionary<string, object?>(headers)
            };

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            timeout.CancelAfter(TimeSpan.FromSeconds(30));

            await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, mandatory: true,
                basicProperties: properties, body: body, cancellationToken: timeout.Token);
        }
        finally
        {
            _publishLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        // Reject new work immediately, then wait for the active publish before closing its channel.
        Volatile.Write(ref _disposeRequested, 1);
        await _publishLock.WaitAsync();
        try
        {
            var channel = _channel;
            _channel = null;
            if (channel != null) await channel.DisposeAsync();
        }
        finally
        {
            _publishLock.Release();
        }
        // Keep the managed semaphore usable for queued callers and repeated disposal through DI aliases.
        // Its AvailableWaitHandle is never accessed, so no native wait handle needs disposing.
    }
}
