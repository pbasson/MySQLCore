using MySQLCore.Core.Interfaces.Messager;
using MySQLCore.Infrastructure.Messager;

namespace MySQLCore.API.Configurations;

public static class RegisterServices {

    public static IServiceCollection RegisterService(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        RegisterCoreServices(services);
        RegisterCoreRepos(services);

        return services;
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddScoped<ICRUDTransactionService,CRUDTransactionService>();
        services.AddScoped<IImageTransactionService,ImageTransactionService>();
        services.AddScoped<IMessagePublisher,RabbitMQPublisher>();
        services.AddScoped<ProcessMessagePublisher>();
    }

    private static void RegisterCoreRepos(IServiceCollection services)
    {
        services.AddScoped<ICRUDTransactionRepo, CRUDTransactionRepo>();
        services.AddScoped<IImageTransactionRepo, ImageTransactionRepo>();
        services.AddScoped<IProcessedMessageRepo, ProcessedMessageRepo>();
    }
        
}