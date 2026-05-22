namespace MySQLCore.Worker.Configurations;

public static class RegisterDatabases
{
    public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        SetDBConnection<MySQLCoreDBContext>(services, configuration);
        return services;        
    }

    private static void SetDBConnection<TDBContext>(IServiceCollection services, IConfiguration configuration) where TDBContext : DbContext
    {
        var setDB = SetConnectionString(configuration);

        services.AddDbContext<TDBContext>( db => {
            db.UseMySql(setDB, new MySqlServerVersion(new Version(8, 3, 0)),
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

}
