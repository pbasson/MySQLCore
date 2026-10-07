namespace MySQLCore.Worker.Configurations;

public static class RegisterMessagers
{
    public static IServiceCollection RegisterMessager(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterProvider(services, configuration);
        RegisterBackgroundServices(services);

        return services;
    }
    
    public static IServiceCollection RegisterProvider(IServiceCollection services, IConfiguration configuration)
    {
        switch (configuration["Messaging:Provider"])
        {
            case "RabbitMQ":
                services.Configure<RabbitMQSetting>(configuration.GetSection("RabbitMQ"));
                services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
                services.AddSingleton<IRabbitMQBus, RabbitMQBus>();
                services.AddHostedService<RabbitMQImageWorker>();
                break;

            case "AzureServiceBus":
                services.AddOptions<AzureServiceBusSetting>()
                    .Bind(configuration.GetSection("Messaging:AzureServiceBus"))
                    .Validate(settings => !string.IsNullOrWhiteSpace(settings.QueueName),
                        "Messaging:AzureServiceBus:QueueName is required.")
                    .Validate(settings => !string.IsNullOrWhiteSpace(settings.FullyQualifiedNamespace),
                        "Messaging:AzureServiceBus:FullyQualifiedNamespace is required.")
                    .Validate(settings => Uri.CheckHostName(settings.FullyQualifiedNamespace) == UriHostNameType.Dns,
                        "Azure Service Bus namespace must be a hostname without a scheme or path.")
                    .ValidateOnStart();

                services.AddSingleton<IMessageBus, AzureServiceBus>();
                services.AddHostedService<AzureServiceBusImageWorker>();

                services.AddSingleton<ServiceBusClient>(provider =>
                {
                    var settings = provider.GetRequiredService<IOptions<AzureServiceBusSetting>>().Value;
                    return new ServiceBusClient(settings.FullyQualifiedNamespace, new DefaultAzureCredential());
                });

                break;

            default:
                throw new InvalidOperationException("Messaging:Provider is Required.");
        }

        return services;
    }

    private static void RegisterBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<OutboxWorker>();
    }
}