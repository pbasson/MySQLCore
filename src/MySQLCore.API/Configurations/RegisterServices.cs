using MySQLCore.Core.Interfaces.Messager.Repo;
using MySQLCore.Core.Interfaces.Messager.Service;
using MySQLCore.Core.Interfaces.Repos.ImageGallery;
using MySQLCore.Core.Interfaces.Repos.User;
using MySQLCore.Core.Interfaces.Services.ImageGallery;
using MySQLCore.Core.Interfaces.Services.User;
using MySQLCore.Core.Messager.Services;
using MySQLCore.Core.Services.ImageGallery;
using MySQLCore.Core.Services.User;

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
        services.AddScoped<IImageGalleryService,ImageGalleryService>();
        services.AddScoped<ICacheService,RedisCacheService>();
        services.AddScoped<IProcessedMessageService,ProcessedMessageService>();
        services.AddScoped<IOutboxMessagerService,OutboxMessagerService>();
    }

    private static void RegisterCoreRepos(IServiceCollection services)
    {
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IImageGalleryRepo, ImageGalleryRepo>();
        services.AddScoped<IProcessedMessageRepo, ProcessedMessageRepo>();
        services.AddScoped<IOutboxMessagerRepo, OutboxMessagerRepo>();
    }
}