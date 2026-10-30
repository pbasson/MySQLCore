using MySQLCore.Core.Worker.Configurations;

namespace MySQLCore.Consumer.Worker.Configurations;

public static class RegisterMessagers
{
    public static IServiceCollection RegisterMessager(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterBaseMessagers.RegisterProvider(services, configuration);

        switch (configuration["Messaging:Provider"])
        {
            case "RabbitMQ":
                services.AddHostedService<RabbitMQImageWorker>();
                break;
            case "AzureServiceBus":
                services.AddHostedService<AzureServiceBusImageWorker>();
                break;
        }

        return services;
    }
}
