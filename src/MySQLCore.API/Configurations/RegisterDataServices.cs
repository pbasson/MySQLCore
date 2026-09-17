namespace MySQLCore.API.Configurations;

public static class RegisterDataServices
{
    public static IServiceCollection RegisterData(this IServiceCollection services, IConfiguration configuration)
    {
        SetDBConnection<MySQLCoreDBContext>(services, configuration);
        RegisterCache(services, configuration);
        RegisterHealthChecks(services, configuration);
        return services;        
    }

    private static void SetDBConnection<TDBContext>(IServiceCollection services, IConfiguration configuration) where TDBContext : DbContext
    {
        var setDB = SetConnectionString(configuration);

        services.AddDbContext<TDBContext>( db => {
            db.UseMySql(setDB,ServerVersion.AutoDetect(setDB),
            db => db.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null));
        });
    }

    private static string SetConnectionString(IConfiguration _configuration)
    {
        var host = _configuration[AppSettings.DB_Host] ?? _configuration.GetConnectionString(AppSettings.DB_Host) ;
        var port = _configuration[AppSettings.DB_Port] ?? _configuration.GetConnectionString(AppSettings.DB_Port) ;
        var dataBase = _configuration[AppSettings.MySQL_Database] ?? _configuration.GetConnectionString(AppSettings.MySQL_Database);
        var userid = _configuration[AppSettings.MySQL_Root_User] ?? _configuration.GetConnectionString(AppSettings.MySQL_Root_User);
        var password = _configuration[AppSettings.MySQL_Root_Password] ?? _configuration.GetConnectionString(AppSettings.MySQL_Root_Password);

        return $"server={host}; database={dataBase}; port={port}; userid={userid}; pwd={password};";
    }

    private static void RegisterCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetValue<string>("Redis:Connection")
            ?? throw new InvalidOperationException("Redis connection is not configured.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "MySQLCore:";
        });
    }

    private static void RegisterHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = SetConnectionString(configuration);
        services.AddHealthChecks().AddMySql(connectionString, name: "mysql", tags: ["ready"]);
    }
}
