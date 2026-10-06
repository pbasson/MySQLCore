namespace MySQLCore.Worker.Messager;

public sealed class RabbitMQBus : IMessageBus, IAsyncDisposable
{
    private readonly ILogger<RabbitMQBus> _logger;
    private readonly RabbitMQService _service;
    private IChannel? _channel;
    private readonly SemaphoreSlim _publishLock = new(1, 1);

    public RabbitMQBus(ILogger<RabbitMQBus> logger, RabbitMQService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        using Activity? activity = TracingConstants.StartMessagingActivity<RabbitMQBus>(nameof(PublishAsync));
        activity?.SetTag("queue.name", queueName);
        activity?.SetTag("message.type", typeof(TMessage).Name);

        byte[] body = _service.SerializeMessage(message);

        await ForwardAsync(queueName, body, headers: null, cancellationToken: cancellationToken);
        
        _logger.LogInformation("{QueueName} Message Status: {status} ", queueName, nameof(ProcessMessageStatus.Published));
        MessageMetric.Published.Inc();
    }

    public async Task ForwardAsync(string queueName, ReadOnlyMemory<byte> body, IDictionary<string, object?>? headers, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);

        await _publishLock.WaitAsync(cancellationToken);
        try
        {
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
        if (_channel != null) await _channel.DisposeAsync();
        _publishLock.Dispose();
    }
}
