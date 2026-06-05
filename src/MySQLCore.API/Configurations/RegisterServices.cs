namespace MySQLCore.API.Configurations;

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
        services.AddScoped<IUserService,UserService>();
        services.AddScoped<IImageTransactionService,ImageTransactionService>();
        services.AddScoped<ICacheService,RedisCacheService>();
        services.AddScoped<IProcessedMessageService,ProcessedMessageService>();
        services.AddScoped<IOutboxMessagerService,OutboxMessagerService>();
    }

    private static void RegisterCoreRepos(IServiceCollection services)
    {
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IImageTransactionRepo, ImageTransactionRepo>();
        services.AddScoped<IProcessedMessageRepo, ProcessedMessageRepo>();
        services.AddScoped<IOutboxMessagerRepo, OutboxMessagerRepo>();
    }
}