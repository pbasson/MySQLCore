using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using MySQLCore.Core.Constants;
using MySQLCore.Core.Interfaces.Messager.Models;
using MySQLCore.Worker.Messager.Azure;
using Xunit;

namespace MySQLCore.Messaging.IntegrationTest;

public class AzurePublisherTests
{
    [Fact]
    public async Task SuccessIsReportedOnlyAfterSendCompletes()
    {
        var client = new FakeClient();
        var logger = new CaptureLogger();
        await using var bus = new AzureServiceBus(logger, client);
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Send = (_, token) => gate.Task.WaitAsync(token);
        var message = new TestMessage(Guid.NewGuid(), "image.jpg");
        var publish = bus.PublishAsync("images", message);
        Assert.False(publish.IsCompleted);
        Assert.Empty(logger.Messages);
        var sender = Assert.Single(client.Senders).Value;
        Assert.Equal(message.MessageId.ToString(), sender.Message!.MessageId);
        Assert.Equal("application/json", sender.Message.ContentType);
        Assert.Equal(message, JsonSerializer.Deserialize<TestMessage>(sender.Message.Body.ToString()));
        gate.SetResult();
        await publish;
        Assert.Contains("Published", Assert.Single(logger.Messages));
    }

    [Fact]
    public async Task FailedSendPropagatesAndMarksProducerActivityWithoutLoggingSuccess()
    {
        using var root = new Activity("publisher-test").SetIdFormat(ActivityIdFormat.W3C).Start();
        var spans = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == TracingConstants.ACTIVITY_SOURCE,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => { if (activity.TraceId == root.TraceId) spans.Add(activity); }
        };
        ActivitySource.AddActivityListener(listener);
        var failure = new InvalidOperationException("Send failed");
        var client = new FakeClient { Send = (_, _) => Task.FromException(failure) };
        var logger = new CaptureLogger();
        await using var bus = new AzureServiceBus(logger, client);
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            bus.PublishAsync("images", new TestMessage(Guid.NewGuid(), "image.jpg")));
        Assert.Same(failure, actual);
        Assert.Empty(logger.Messages);
        var span = Assert.Single(spans);
        Assert.Equal("AzureServiceBus.PublishAsync", span.OperationName);
        Assert.Equal(ActivityKind.Producer, span.Kind);
        Assert.Equal(ActivityStatusCode.Error, span.Status);
    }

    [Fact]
    public async Task ReusesSendersPerQueueAndDisposesOnlyOwnedResources()
    {
        var client = new FakeClient();
        var bus = new AzureServiceBus(new CaptureLogger(), client);
        var message = new TestMessage(Guid.NewGuid(), "image.jpg");
        await bus.PublishAsync("images", message);
        await bus.PublishAsync("images", message);
        await bus.PublishAsync("other", message);
        Assert.Equal(2, client.Senders.Count);
        Assert.Equal(2, client.Senders["images"].Calls);
        await bus.DisposeAsync();
        await bus.DisposeAsync();
        Assert.All(client.Senders.Values, sender => Assert.Equal(1, sender.Disposals));
        Assert.False(client.Disposed);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => bus.PublishAsync("images", message));
    }

    [Theory]
    [InlineData("queue")]
    [InlineData("message")]
    [InlineData("id")]
    public async Task RejectsInvalidInputBeforeCreatingSender(string invalid)
    {
        var client = new FakeClient();
        await using var bus = new AzureServiceBus(new CaptureLogger(), client);
        var message = invalid == "message" ? null : new TestMessage(invalid == "id" ? Guid.Empty : Guid.NewGuid(), "image.jpg");
        await Assert.ThrowsAnyAsync<ArgumentException>(() => bus.PublishAsync(invalid == "queue" ? " " : "images", message!));
        Assert.Empty(client.Senders);
    }

    [Fact]
    public async Task CancellationDuringSendPropagatesWithoutReportingSuccess()
    {
        var logger = new CaptureLogger();
        var client = new FakeClient { Send = (_, token) => Task.Delay(Timeout.InfiniteTimeSpan, token) };
        await using var bus = new AzureServiceBus(logger, client);
        using var cancellation = new CancellationTokenSource();
        var publish = bus.PublishAsync("images", new TestMessage(Guid.NewGuid(), "image.jpg"), cancellation.Token);
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => publish);
        Assert.Empty(logger.Messages);
    }

    public record TestMessage(Guid MessageId, string FileName) : IMessage;

    private sealed class CaptureLogger : ILogger<AzureServiceBus>
    {
        public List<string> Messages { get; } = new();
        public bool IsEnabled(LogLevel level) => true;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
    }

    private sealed class FakeClient : ServiceBusClient
    {
        public Dictionary<string, FakeSender> Senders { get; } = new();
        public Func<ServiceBusMessage, CancellationToken, Task> Send = (_, _) => Task.CompletedTask;
        public bool Disposed;
        public override ServiceBusSender CreateSender(string queueName)
        {
            var sender = new FakeSender(Send);
            Senders.Add(queueName, sender);
            return sender;
        }
        public override ValueTask DisposeAsync()
        {
            Disposed = true;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class FakeSender(Func<ServiceBusMessage, CancellationToken, Task> send) : ServiceBusSender
    {
        public ServiceBusMessage? Message;
        public int Calls;
        public int Disposals;
        public override Task SendMessageAsync(ServiceBusMessage message, CancellationToken cancellationToken = default)
        {
            Message = message;
            Calls++;
            return send(message, cancellationToken);
        }
        public override ValueTask DisposeAsync()
        {
            Disposals++;
            return ValueTask.CompletedTask;
        }
    }
}
