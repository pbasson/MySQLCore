using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MySQLCore.Core.Constants;
using MySQLCore.Core.Enums;
using MySQLCore.Core.Interfaces.Messager.Repo;
using MySQLCore.Worker.BackgroundServices.ImageGallery;
using MySQLCore.Worker.Constants.Settings;
using MySQLCore.Worker.Interfaces.Worker;
using MySQLCore.Worker.Messager;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace MySQLCore.Messaging.IntegrationTest;

public class RabbitMQTracingTests
{
    [Theory]
    [InlineData("success")]
    [InlineData("duplicate")]
    [InlineData("processing-failure")]
    [InlineData("ack-failure")]
    [InlineData("forward-failure")]
    [InlineData("invalid")]
    [InlineData("cancel")]
    public async Task DeliveryActivityCoversProcessingAndSettlement(string scenario)
    {
        using var root = new Activity("trace-test").SetIdFormat(ActivityIdFormat.W3C).Start();
        var stopped = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == TracingConstants.ACTIVITY_SOURCE,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity =>
            {
                if (activity.TraceId == root.TraceId) stopped.Add(activity);
            }
        };
        ActivitySource.AddActivityListener(listener);
        using var cancellation = new CancellationTokenSource();
        Activity? duringProcessing = null;
        Activity? duringAck = null;
        Activity? duringForward = null;
        var processingCalls = 0;
        var repo = Proxy<IProcessedMessageRepo>((method, _) =>
        {
            Assert.Equal(nameof(IProcessedMessageRepo.ProcessImageCreatedAsync), method.Name);
            duringProcessing = Activity.Current;
            processingCalls++;
            if (scenario == "cancel")
            {
                cancellation.Cancel();
                return Task.FromCanceled<MessageProcessResult>(cancellation.Token);
            }
            if (scenario is "processing-failure" or "forward-failure")
                return Task.FromException<MessageProcessResult>(new InvalidOperationException("Database failed"));
            return Task.FromResult(scenario == "duplicate" ? MessageProcessResult.Duplicate : MessageProcessResult.Completed);
        });
        var channel = Proxy<IChannel>((method, _) =>
        {
            Assert.Equal(nameof(IChannel.BasicAckAsync), method.Name);
            duringAck = Activity.Current;
            return scenario == "ack-failure"
                ? ValueTask.FromException(new InvalidOperationException("Ack failed")) : ValueTask.CompletedTask;
        });
        var bus = Proxy<IMessageBus>((method, _) =>
        {
            Assert.Equal(nameof(IMessageBus.ForwardAsync), method.Name);
            duringForward = Activity.Current;
            return scenario == "forward-failure"
                ? Task.FromException(new InvalidOperationException("Forward failed")) : Task.CompletedTask;
        });
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(repo);
        services.AddScoped<ProcessWorkerService>();
        await using var provider = services.BuildServiceProvider();
        using var worker = new RabbitMQImageWorker(NullLogger<RabbitMQImageWorker>.Instance,
            provider.GetRequiredService<IServiceScopeFactory>(), Options.Create(new RabbitMQSetting
            {
                RetryQueueName = "retry", DeadLetterQueueName = "dead"
            }), null!, bus);
        var body = scenario == "invalid" ? "not-json" : "{\"MessageId\":\"5bc60a6d-36da-463f-bb48-8da6bf314391\"}";
        var delivery = new BasicDeliverEventArgs("consumer", 1, false, "", "images",
            new BasicProperties(), Encoding.UTF8.GetBytes(body), cancellation.Token);
        var method = typeof(RabbitMQImageWorker).GetMethod("HandleMessageAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Task Handle() => (Task)method.Invoke(worker, new object[] { delivery, channel, cancellation.Token })!;
        if (scenario is "ack-failure" or "forward-failure")
            await Assert.ThrowsAsync<InvalidOperationException>(Handle);
        else if (scenario == "cancel")
            await Assert.ThrowsAnyAsync<OperationCanceledException>(Handle);
        else
            await Handle();

        var outer = Assert.Single(stopped, a => a.Kind == ActivityKind.Consumer);
        Assert.Equal("RabbitMQImageWorker.HandleMessageAsync", outer.OperationName);
        var failed = scenario is "processing-failure" or "ack-failure" or "forward-failure" or "invalid";
        Assert.Equal(failed ? ActivityStatusCode.Error : ActivityStatusCode.Unset, outer.Status);
        Assert.Equal(scenario == "invalid" ? 0 : 1, processingCalls);
        if (scenario != "invalid")
        {
            var inner = Assert.Single(stopped, a => a.OperationName == "ProcessWorkerService.ProcessAsync");
            Assert.Same(inner, duringProcessing);
            Assert.Equal(outer.SpanId, inner.ParentSpanId);
            Assert.Equal(scenario is "processing-failure" or "forward-failure"
                ? ActivityStatusCode.Error : ActivityStatusCode.Unset, inner.Status);
            Assert.True(stopped.IndexOf(inner) < stopped.IndexOf(outer));
        }
        if (scenario is "processing-failure" or "forward-failure" or "invalid")
            Assert.Same(outer, duringForward);
        else
            Assert.Null(duringForward);
        if (scenario is "cancel" or "forward-failure")
            Assert.Null(duringAck);
        else
            Assert.Same(outer, duringAck);
        Assert.Equal(scenario == "invalid" ? 1 : 2, stopped.Count);
    }

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
    {
        var proxy = DispatchProxy.Create<T, CallProxy>();
        ((CallProxy)(object)proxy).Handler = handler;
        return proxy;
    }

    public class CallProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Handler = null!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args);
    }
}
