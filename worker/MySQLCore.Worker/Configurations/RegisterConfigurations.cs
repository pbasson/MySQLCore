namespace MySQLCore.Worker.Configurations;

public static class RegisterConfigurations
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        RegisterMessager(services, configuration);
        services.RegisterDatabase(configuration);
        services.RegisterService();
        RegisterBackgroundServices(services);
        return services;
    }

    private static void RegisterMessager(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
    }

    private static void RegisterBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<OutboxPublisherWorker>();
        services.AddHostedService<ImageProcessingWorker>();
    }
}
