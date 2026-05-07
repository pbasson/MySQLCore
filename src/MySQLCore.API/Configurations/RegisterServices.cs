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
        services.AddScoped<ProcessMessageService>();
    }

    private static void RegisterCoreRepos(IServiceCollection services)
    {
        services.AddScoped<ICRUDTransactionRepo, CRUDTransactionRepo>();
        services.AddScoped<IImageTransactionRepo, ImageTransactionRepo>();
        services.AddScoped<IProcessedMessageRepo, ProcessedMessageRepo>();
    }
}