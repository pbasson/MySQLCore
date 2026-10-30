namespace MySQLCore.Core.Worker.Configurations;

public static class RegisterBaseMessagers
{
    public static IServiceCollection RegisterProvider(this IServiceCollection services, IConfiguration configuration)
    {
        switch (configuration["Messaging:Provider"])
        {
            case "RabbitMQ":
                services.Configure<RabbitMQSetting>(configuration.GetSection("RabbitMQ"));
                services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
                services.AddSingleton<IRabbitMQBus, RabbitMQBus>();
                services.AddSingleton<IMessageBus>(provider => provider.GetRequiredService<IRabbitMQBus>());
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

}
