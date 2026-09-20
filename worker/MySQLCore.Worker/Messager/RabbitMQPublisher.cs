namespace MySQLCore.Worker.Messager;

public sealed class RabbitMQPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly SemaphoreSlim _publishLock = new(1, 1);
    private IChannel? _channel;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly RabbitMQConnectionService _connectionService;
    public RabbitMQPublisher(ILogger<RabbitMQPublisher> logger, RabbitMQConnectionService connectionService)
    {
        _logger = logger;
        _connectionService = connectionService;
    }

    public async Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        using Activity? activity = TracingConstants.StartMessagingActivity<RabbitMQPublisher>(nameof(PublishAsync));
        activity?.SetTag("queue.name", queueName);
        activity?.SetTag("message.type", typeof(TMessage).Name);

        byte[] body = _connectionService.SerializeMessage(message);

        await ForwardAsync(queueName, body, headers: null, cancellationToken: cancellationToken);
        
        _logger.LogInformation("{QueueName} Message Status: {status} ", queueName, nameof(ProcessMessageStatus.Published));
        MessageMetrics.Published.Inc();
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
                _channel = await _connectionService.CreateChannelAsync(cancellationToken, publisherConfirmations: true);
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
