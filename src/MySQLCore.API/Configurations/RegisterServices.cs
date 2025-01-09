using System;
using AutoMapper;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Interfaces.InterfaceServices;
using MySQLCore.Core.Services;
using MySQLCore.Infrastructure.Repos;

namespace MySQLCore.API.Configurations;

public static class RegisterServices
{
    public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        #region Register Automapper
        var mappingConfig = new MapperConfiguration( map => map.AddProfile(new MappingProfile()));
        services.AddSingleton(mappingConfig.CreateMapper());
        #endregion

        #region Register Core Services
        RegisterCoreServices(services);
        #endregion

        #region Register Core Repos
        RegisterCoreRepos(services);
        #endregion

        return services;
    }
    
    
    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddScoped<ICRUDTransactionService,CRUDTransactionService>();
    }

    private static void RegisterCoreRepos(IServiceCollection services)
    {
        services.AddScoped<ICRUDTransactionRepo, CRUDTransactionRepo>();
    }
}
