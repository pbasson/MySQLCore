// using System.Diagnostics;
// using System.Reflection;
// using System.Text;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using MySQLCore.Core.Constants;
// using MySQLCore.Core.Interfaces.Messager.Models;
// using MySQLCore.Worker.Configurations;
// using MySQLCore.Worker.Interfaces.Worker;
// using MySQLCore.Worker.Messager.RabbitMQ;
// using RabbitMQ.Client;
// using Xunit;

// namespace MySQLCore.Messaging.IntegrationTest;

// public class RabbitMQPublisherTests
// {
//     [Theory]
//     [InlineData("queue")]
//     [InlineData("message")]
//     [InlineData("id")]
//     public async Task InvalidInputNeverCreatesChannel(string invalid)
//     {
//         var connection = new Connection();
//         await using var bus = new RabbitMQBus(new CaptureLogger(), connection);
//         var message = invalid == "message" ? null : new Message(invalid == "id" ? Guid.Empty : Guid.NewGuid());
//         await Assert.ThrowsAnyAsync<ArgumentException>(() => bus.PublishAsync(invalid == "queue" ? " " : "images", message!));
//         Assert.Equal(0, connection.Creates);
//     }

//     [Fact]
//     public async Task WaitsForConfirmationBeforeReportingSuccessAndReusesChannel()
//     {
//         var connection = new Connection();
//         var log = new CaptureLogger();
//         await using var bus = new RabbitMQBus(log, connection);
//         var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
//         connection.State.Send = token => new ValueTask(gate.Task.WaitAsync(token));
//         var publish = bus.PublishAsync("images", new Message(Guid.NewGuid()));
//         Assert.False(publish.IsCompleted);
//         Assert.Empty(log.Messages);
//         gate.SetResult();
//         await publish;
//         await bus.PublishAsync("images", new Message(Guid.NewGuid()));
//         Assert.Equal(2, log.Messages.Count);
//         Assert.Equal(1, connection.Creates);
//         Assert.True(connection.Confirmations);
//     }

//     [Fact]
//     public async Task SendFailurePropagatesAndMarksProducerSpanWithoutSuccessLog()
//     {
//         using var root = new Activity("rabbit-publish-test").SetIdFormat(ActivityIdFormat.W3C).Start();
//         var spans = new List<Activity>();
//         using var listener = new ActivityListener
//         {
//             ShouldListenTo = source => source.Name == TracingConstants.ACTIVITY_SOURCE,
//             Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
//             ActivityStopped = activity => { if (activity.TraceId == root.TraceId) spans.Add(activity); }
//         };
//         ActivitySource.AddActivityListener(listener);
//         var connection = new Connection();
//         connection.State.Send = _ => ValueTask.FromException(new InvalidOperationException("Broker rejected send"));
//         var log = new CaptureLogger();
//         await using var bus = new RabbitMQBus(log, connection);
//         await Assert.ThrowsAsync<InvalidOperationException>(() => bus.PublishAsync("images", new Message(Guid.NewGuid())));
//         Assert.Empty(log.Messages);
//         var span = Assert.Single(spans);
//         Assert.Equal(ActivityKind.Producer, span.Kind);
//         Assert.Equal(ActivityStatusCode.Error, span.Status);
//         // Failure must release the publishing lock for later attempts.
//         connection.State.Send = _ => ValueTask.CompletedTask;
//         await bus.PublishAsync("images", new Message(Guid.NewGuid()));
//         Assert.Single(log.Messages);
//     }

//     [Fact]
//     public async Task RawForwardingPreservesInvalidPayloadAndHeaders()
//     {
//         var connection = new Connection();
//         await using var bus = new RabbitMQBus(new CaptureLogger(), connection);
//         var body = Encoding.UTF8.GetBytes("not-json");
//         var headers = new Dictionary<string, object?> { ["x-retry-count"] = 2 };
//         await bus.ForwardAsync("dead", body, headers);
//         Assert.Equal(body, connection.State.Body.ToArray());
//         Assert.Equal(2, connection.State.Properties!.Headers!["x-retry-count"]);
//         Assert.NotSame(headers, connection.State.Properties.Headers);
//         Assert.True(connection.State.Properties.Persistent);
//     }

//     [Fact]
//     public async Task DisposalWaitsForPublishRejectsQueuedWorkAndIsRepeatSafe()
//     {
//         var connection = new Connection();
//         var bus = new RabbitMQBus(new CaptureLogger(), connection);
//         var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
//         connection.State.Send = _ => new ValueTask(gate.Task);
//         var first = bus.PublishAsync("images", new Message(Guid.NewGuid()));
//         var queued = bus.PublishAsync("images", new Message(Guid.NewGuid()));
//         var dispose = bus.DisposeAsync().AsTask();
//         Assert.False(dispose.IsCompleted);
//         Assert.Equal(0, connection.State.Disposals);
//         gate.SetResult();
//         await first;
//         await Assert.ThrowsAsync<ObjectDisposedException>(() => queued);
//         await dispose;
//         await bus.DisposeAsync();
//         Assert.Equal(1, connection.State.Disposals);
//         await Assert.ThrowsAsync<ObjectDisposedException>(() => bus.PublishAsync("images", new Message(Guid.NewGuid())));
//         await Assert.ThrowsAsync<ObjectDisposedException>(() => bus.ForwardAsync("dead", ReadOnlyMemory<byte>.Empty, null));
//     }

//     [Fact]
//     public async Task CancellationReleasesLockWithoutReportingSuccess()
//     {
//         var connection = new Connection();
//         connection.State.Send = token => new ValueTask(Task.Delay(Timeout.InfiniteTimeSpan, token));
//         var log = new CaptureLogger();
//         await using var bus = new RabbitMQBus(log, connection);
//         using var cancellation = new CancellationTokenSource();
//         var publish = bus.PublishAsync("images", new Message(Guid.NewGuid()), cancellation.Token);
//         cancellation.Cancel();
//         await Assert.ThrowsAnyAsync<OperationCanceledException>(() => publish);
//         Assert.Empty(log.Messages);
//         connection.State.Send = _ => ValueTask.CompletedTask;
//         await bus.PublishAsync("images", new Message(Guid.NewGuid()));
//         Assert.Single(log.Messages);
//     }

//     [Fact]
//     public async Task BothInterfacesResolveSameInstanceAndContainerDisposalClosesChannelOnce()
//     {
//         var services = new ServiceCollection();
//         services.AddLogging();
//         var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
//         {
//             ["Messaging:Provider"] = "RabbitMQ"
//         }).Build();
//         RegisterMessagers.RegisterProvider(services, configuration);
//         var connection = new Connection();
//         services.AddSingleton<IRabbitMQConnection>(connection);
//         var provider = services.BuildServiceProvider();
//         try
//         {
//             var shared = provider.GetRequiredService<IMessageBus>();
//             Assert.Same(shared, provider.GetRequiredService<IRabbitMQBus>());
//             await shared.PublishAsync("images", new Message(Guid.NewGuid()));
//         }
//         finally { await provider.DisposeAsync(); }
//         Assert.Equal(1, connection.State.Disposals);
//     }

//     public record Message(Guid MessageId) : IMessage;
//     private sealed class CaptureLogger : ILogger<RabbitMQBus>
//     {
//         public List<string> Messages { get; } = new();
//         public bool IsEnabled(LogLevel level) => true;
//         public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
//         public void Log<TState>(LogLevel level, EventId id, TState state, Exception? exception,
//             Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
//     }

//     private sealed class Connection : IRabbitMQConnection
//     {
//         private readonly IChannel _channel = DispatchProxy.Create<IChannel, ChannelProxy>();
//         public ChannelProxy State => (ChannelProxy)_channel;
//         public int Creates;
//         public bool Confirmations;
//         public Task<IChannel> CreateChannelAsync(CancellationToken stoppingToken, bool publisherConfirmations = false)
//         {
//             Creates++;
//             Confirmations = publisherConfirmations;
//             return Task.FromResult(_channel);
//         }
//     }

//     public class ChannelProxy : DispatchProxy
//     {
//         public Func<CancellationToken, ValueTask> Send = _ => ValueTask.CompletedTask;
//         public int Disposals;
//         public ReadOnlyMemory<byte> Body;
//         public BasicProperties? Properties;
//         protected override object? Invoke(MethodInfo? method, object?[]? args)
//         {
//             switch (method!.Name)
//             {
//                 case "get_IsOpen": return true;
//                 case "DisposeAsync": Disposals++; return ValueTask.CompletedTask;
//                 case "BasicPublishAsync":
//                     Assert.Equal(string.Empty, args![0]);
//                     Assert.Equal(true, args[2]);
//                     Properties = (BasicProperties)args[3]!;
//                     Body = (ReadOnlyMemory<byte>)args[4]!;
//                     return Send((CancellationToken)args[5]!);
//                 default: throw new NotSupportedException(method.Name);
//             }
//         }
//     }
// }
