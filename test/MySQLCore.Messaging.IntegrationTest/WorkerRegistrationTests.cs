// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using MySQL.Core.Worker.Interfaces.Worker;
// using MySQL.Core.Worker.Messager.Azure;
// using MySQL.Core.Worker.Messager.RabbitMQ;
// using MySQLCore.Consumer.Worker.BackgroundServices.ImageGallery;
// using MySQLCore.Producer.Worker.BackgroundServices.Outbox;
// using Xunit;
// using CommonRegistration = MySQLCore.Core.Worker.Configurations.RegisterBaseMessagers;
// using ConsumerRegistration = MySQLCore.Consumer.Worker.Configurations.RegisterMessagers;
// using ProducerRegistration = MySQLCore.Producer.Worker.Configurations.RegisterMessagers;

// namespace MySQLCore.Messaging.IntegrationTest;

// public class WorkerRegistrationTests
// {
//     [Theory]
//     [InlineData("RabbitMQ")]
//     [InlineData("AzureServiceBus")]
//     public void CommonRegistersTransportWithoutStartingWorkers(string provider)
//     {
//         var services = new ServiceCollection();
//         CommonRegistration.RegisterProvider(services, Configuration(provider));

//         Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IHostedService));
//         Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IMessageBus));
//         if (provider == "RabbitMQ")
//         {
//             Assert.Contains(services, descriptor => descriptor.ImplementationType == typeof(RabbitMQBus));
//             Assert.DoesNotContain(services, descriptor => descriptor.ImplementationType == typeof(AzureServiceBus));
//         }
//         else
//         {
//             Assert.Contains(services, descriptor => descriptor.ImplementationType == typeof(AzureServiceBus));
//             Assert.DoesNotContain(services, descriptor => descriptor.ImplementationType == typeof(RabbitMQBus));
//         }
//     }

//     [Theory]
//     [InlineData("RabbitMQ")]
//     [InlineData("AzureServiceBus")]
//     public void ProducerRegistersOnlyOutboxWorker(string provider)
//     {
//         var services = new ServiceCollection();
//         ProducerRegistration.RegisterMessager(services, Configuration(provider));

//         var workers = services.Where(descriptor => descriptor.ServiceType == typeof(IHostedService)).ToArray();
//         Assert.Equal(typeof(OutboxWorker), Assert.Single(workers).ImplementationType);
//         Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IMessageBus));
//     }

//     [Theory]
//     [InlineData("RabbitMQ", typeof(RabbitMQImageWorker))]
//     [InlineData("AzureServiceBus", typeof(AzureServiceBusImageWorker))]
//     public void ConsumerRegistersOnlySelectedImageWorker(string provider, Type expectedWorker)
//     {
//         var services = new ServiceCollection();
//         ConsumerRegistration.RegisterMessager(services, Configuration(provider));

//         var workers = services.Where(descriptor => descriptor.ServiceType == typeof(IHostedService)).ToArray();
//         Assert.Equal(expectedWorker, Assert.Single(workers).ImplementationType);
//     }

//     private static IConfiguration Configuration(string provider) =>
//         new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
//         {
//             ["Messaging:Provider"] = provider,
//             ["Messaging:AzureServiceBus:QueueName"] = "images",
//             ["Messaging:AzureServiceBus:FullyQualifiedNamespace"] = "test.servicebus.windows.net"
//         }).Build();
// }
