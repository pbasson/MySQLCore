using MySQLCore.Core.Worker.Configurations;

namespace MySQLCore.Producer.Worker.Configurations;

public static class RegisterMessagers
{
    public static IServiceCollection RegisterMessager(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterProvider(configuration);
        services.AddHostedService<OutboxWorker>();

        return services;
    }
}
