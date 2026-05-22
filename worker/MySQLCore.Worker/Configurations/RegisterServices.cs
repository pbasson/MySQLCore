using MySQLCore.Infrastructure.Repos.MessagerRepo;

namespace MySQLCore.Worker.Configurations;

public static class RegisterServices 
{
    public static IServiceCollection RegisterService(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        RegisterCoreServices(services);
        RegisterCoreRepos(services);
        return services;
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddScoped<IMessagePublisher,RabbitMQPublisher>();
        services.AddScoped<ProcessMessageService>();
        services.AddSingleton<RabbitMQConnectionService>();
    }

    private static void RegisterCoreRepos(IServiceCollection services)
    {
        services.AddScoped<IProcessedMessageRepo, ProcessedMessageRepo>();
        services.AddScoped<IOutboxMessagerRepo, OutboxMessagerRepo>();
    }
}