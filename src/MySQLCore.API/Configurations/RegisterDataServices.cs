using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Interfaces.InterfaceServices;
using MySQLCore.Core.Services;
using MySQLCore.Infrastructure.Repos;

namespace MySQLCore.API.Configurations
{
    public static class RegisterDataServices {

        public static IServiceCollection RegisterDataService(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            RegisterCoreServices(services);
            RegisterCoreRepos(services);

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
}