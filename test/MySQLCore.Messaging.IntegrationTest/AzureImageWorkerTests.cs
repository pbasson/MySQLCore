using System.Reflection;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MySQLCore.Core.Enums;
using MySQLCore.Core.Interfaces.Messager.Repo;
using MySQLCore.Worker.BackgroundServices.ImageGallery;
using MySQLCore.Worker.Constants.Settings;
using MySQLCore.Worker.Messager;
using Xunit;

namespace MySQLCore.Messaging.IntegrationTest;

public class AzureImageWorkerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SuccessfulAndDuplicateMessagesComplete(bool duplicate)
    {
        await using var fixture = new Fixture();
        fixture.Repo.Duplicate = duplicate;
        await fixture.StartAsync();
        var delivery = new Delivery(ValidBody);
        await fixture.Client.Processor.DeliverAsync(delivery);
        Assert.Equal(1, fixture.Repo.Calls);
        Assert.Equal(new[] { "complete" }, delivery.Actions);
    }

    [Theory]
    [InlineData("not-json")]
    [InlineData("null")]
    [InlineData("{}")]
    public async Task InvalidMessagesDeadLetterWithoutProcessing(string body)
    {
        await using var fixture = new Fixture();
        await fixture.StartAsync();
        var delivery = new Delivery(body);
        await fixture.Client.Processor.DeliverAsync(delivery);
        Assert.Equal(0, fixture.Repo.Calls);
        Assert.Equal(new[] { "dead-letter" }, delivery.Actions);
    }

    [Fact]
    public async Task ProcessingFailureAbandonsWithoutCompleting()
    {
        await using var fixture = new Fixture();
        fixture.Repo.Fail = true;
        await fixture.StartAsync();
        var delivery = new Delivery(ValidBody);
        await fixture.Client.Processor.DeliverAsync(delivery);
        Assert.Equal(new[] { "abandon" }, delivery.Actions);
    }

    [Fact]
    public async Task CompletionFailureDoesNotReprocessOrExplicitlyAbandon()
    {
        await using var fixture = new Fixture();
        await fixture.StartAsync();
        var delivery = new Delivery(ValidBody) { FailCompletion = true };
        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Client.Processor.DeliverAsync(delivery));
        Assert.Equal(1, fixture.Repo.Calls);
        Assert.Equal(new[] { "complete" }, delivery.Actions);
    }

    [Fact]
    public async Task ShutdownStopsAndDisposesProcessor()
    {
        await using var fixture = new Fixture();
        await fixture.StartAsync();
        Assert.False(fixture.Client.Options!.AutoCompleteMessages);
        Assert.Equal(ServiceBusReceiveMode.PeekLock, fixture.Client.Options.ReceiveMode);
        await fixture.Worker.StopAsync(CancellationToken.None);
        Assert.True(fixture.Client.Processor.Stopped);
        Assert.True(fixture.Client.Processor.Disposed);
    }

    private const string ValidBody = "{\"MessageId\":\"5bc60a6d-36da-463f-bb48-8da6bf314391\"}";

    private sealed class Fixture : IAsyncDisposable
    {
        public FakeClient Client { get; } = new();
        public RepoProxy Repo { get; }
        public AzureServiceBusImageWorker Worker { get; }
        private readonly ServiceProvider _services;

        public Fixture()
        {
            var repo = DispatchProxy.Create<IProcessedMessageRepo, RepoProxy>();
            Repo = (RepoProxy)repo;
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton(repo);
            services.AddScoped<ProcessWorkerService>();
            _services = services.BuildServiceProvider();
            Worker = new AzureServiceBusImageWorker(NullLogger<AzureServiceBusImageWorker>.Instance,
                Client, Options.Create(new AzureServiceBusSetting { QueueName = "images" }),
                _services.GetRequiredService<IServiceScopeFactory>());
        }

        public async Task StartAsync()
        {
            await Worker.StartAsync(CancellationToken.None);
            await Client.Processor.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }
        public async ValueTask DisposeAsync()
        {
            await Worker.StopAsync(CancellationToken.None);
            Worker.Dispose();
            await _services.DisposeAsync();
            await Client.DisposeAsync();
        }
    }

    public class RepoProxy : DispatchProxy
    {
        public bool Duplicate;
        public bool Fail;
        public int Calls;
        protected override object? Invoke(MethodInfo? method, object?[]? args)
        {
            if (method!.Name != nameof(IProcessedMessageRepo.ProcessImageCreatedAsync))
                throw new NotSupportedException(method.Name);
            Calls++;
            if (Fail) return Task.FromException<MessageProcessResult>(new InvalidOperationException("Database unavailable"));
            return Task.FromResult(Duplicate ? MessageProcessResult.Duplicate : MessageProcessResult.Completed);
        }
    }

    private sealed class FakeClient : ServiceBusClient
    {
        public override ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public FakeProcessor Processor { get; } = new();
        public ServiceBusProcessorOptions? Options;
        public override ServiceBusProcessor CreateProcessor(string queueName, ServiceBusProcessorOptions options)
        {
            Assert.Equal("images", queueName);
            Options = options;
            return Processor;
        }
    }

    private sealed class FakeProcessor : ServiceBusProcessor
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public bool Stopped;
        public bool Disposed;
        public Task DeliverAsync(ProcessMessageEventArgs args) => OnProcessMessageAsync(args);
        public override Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            Started.TrySetResult();
            return Task.CompletedTask;
        }
        public override Task StopProcessingAsync(CancellationToken cancellationToken = default)
        {
            Stopped = true;
            return Task.CompletedTask;
        }
        public override Task CloseAsync(CancellationToken cancellationToken = default)
        {
            Disposed = true;
            return Task.CompletedTask;
        }
    }

    private sealed class Delivery : ProcessMessageEventArgs
    {
        public List<string> Actions { get; } = new();
        public bool FailCompletion;
        public Delivery(string body) : base(ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: new BinaryData(body)), null!, CancellationToken.None) { }
        public override Task CompleteMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default)
        {
            Actions.Add("complete");
            return FailCompletion ? Task.FromException(new InvalidOperationException("Connection lost")) : Task.CompletedTask;
        }
        public override Task AbandonMessageAsync(ServiceBusReceivedMessage message,
            IDictionary<string, object>? propertiesToModify = null, CancellationToken cancellationToken = default)
        {
            Actions.Add("abandon");
            return Task.CompletedTask;
        }
        public override Task DeadLetterMessageAsync(ServiceBusReceivedMessage message, string deadLetterReason,
            string? deadLetterErrorDescription = null, CancellationToken cancellationToken = default)
        {
            Actions.Add("dead-letter");
            return Task.CompletedTask;
        }
    }
}
