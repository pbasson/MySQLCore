using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySQLCore.Core.Interfaces.Messager.Models;
using MySQLCore.Core.Interfaces.Messager.Repo;
using MySQLCore.Core.Messager.Models;
using MySQLCore.Worker.BackgroundServices.Outbox;
using MySQLCore.Worker.Configurations;
using MySQLCore.Worker.Interfaces.Worker;
using Xunit;

namespace MySQLCore.Messaging.IntegrationTest;

public class OutboxQueueTests
{
    [Theory]
    [InlineData("AzureServiceBus", "custom-image-queue", "custom-image-queue")]
    [InlineData("RabbitMQ", null, "image-processing-queue")]
    public async Task PublishesPendingMessageToSelectedProvidersQueue(
        string messagingProvider, string? azureQueue, string expectedQueue)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Messaging:Provider"] = messagingProvider,
            ["Messaging:AzureServiceBus:QueueName"] = azureQueue,
            ["Messaging:AzureServiceBus:FullyQualifiedNamespace"] = "test.servicebus.windows.net"
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.RegisterMessager(configuration);
        var bus = new CapturingBus();
        var repo = DispatchProxy.Create<IOutboxMessagerRepo, OutboxRepo>();
        services.AddSingleton<IMessageBus>(bus);
        services.AddSingleton(repo);
        await using var provider = services.BuildServiceProvider();
        var worker = provider.GetServices<IHostedService>().OfType<OutboxWorker>().Single();
        await worker.StartAsync(CancellationToken.None);
        try
        {
            await ((OutboxRepo)(object)repo).Published.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(expectedQueue, bus.Queue);
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }
    }

    private sealed class CapturingBus : IMessageBus
    {
        public string? Queue { get; private set; }
        public Task PublishAsync<TMessage>(string queueName, TMessage message,
            CancellationToken cancellationToken = default) where TMessage : IMessage
        {
            Queue = queueName;
            return Task.CompletedTask;
        }
    }

    public class OutboxRepo : DispatchProxy
    {
        public TaskCompletionSource Published { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        protected override object? Invoke(MethodInfo? method, object?[]? args)
        {
            if (method!.Name == nameof(IOutboxMessagerRepo.GetPendingAsync))
            {
                var message = new ImageGalleryMessage(1, "image.jpg") { MessageId = Guid.NewGuid() };
                return Task.FromResult(new List<OutboxMessage>
                {
                    new() { Id = 1, MessageId = message.MessageId, Payload = JsonSerializer.Serialize(message) }
                });
            }
            if (method.Name == nameof(IOutboxMessagerRepo.MarkPublishedAsync))
            {
                Published.TrySetResult();
                return Task.CompletedTask;
            }
            throw new NotSupportedException(method.Name);
        }
    }
}
