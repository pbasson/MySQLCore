namespace MySQLCore.Producer.Worker.Configurations;

public static class RegisterMessagers
{
    public static IServiceCollection RegisterMessager(this IServiceCollection services, IConfiguration configuration)
    {
        MySQL.Core.Worker.Configurations.RegisterMessagers.RegisterProvider(services, configuration);
        services.AddHostedService<OutboxWorker>();

        return services;
    }
}
