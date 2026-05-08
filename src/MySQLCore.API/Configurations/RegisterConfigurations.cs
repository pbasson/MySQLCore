namespace MySQLCore.API.Configurations;

public static class RegisterConfigurations
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console()
            .WriteTo.File(
                path: "/Logs/mysqlcore-log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .WriteTo.Seq("http://seq")
            .CreateLogger();


        services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddEndpointsApiExplorer();

        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));

        #region Register Services
        RegisterSwagger(services);
        RegisterLogs(services, configuration);
        #endregion

        #region Register DataService
        services.RegisterService();
        #endregion

        #region Register Database
        services.RegisterDatabase(configuration);
        #endregion

        #region Register Background Services
        services.RegisterBackgroundServices();
        #endregion


        return services;
    }

    public static ConfigureHostBuilder RegisterHost(this ConfigureHostBuilder configure)
    {
        configure.UseSerilog();

        return configure;
    }


    private static void RegisterLogs(IServiceCollection services, IConfiguration configuration)
    {
        services.AddElmah<XmlFileErrorLog>(option => 
        {
            option.LogPath = configuration.GetValue<string>("ElmahPath") ;
        });
    }

    private static void RegisterSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen( x => {
            x.SwaggerDoc("v1", new OpenApiInfo{ Title = "MySQL Core System", Version = "v1"}); 
            x.AddSecurityDefinition(AppSettings.API_KEY, new OpenApiSecurityScheme() {
                Description = $"{AppSettings.API_KEY} Required",
                Name = AppSettings.API_KEY,
                Scheme = "ApiScheme",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });   
            x.AddSecurityRequirement(new () { { new () { 
                Reference = new () { Type = ReferenceType.SecurityScheme, Id = AppSettings.API_KEY },
                In = ParameterLocation.Header }, []
                }
            });
            }
        );
    }

    private static void RegisterBackgroundServices(this IServiceCollection services)
    {
        // services.AddHostedService<Worker>();
        services.AddHostedService<ImageProcessingWorker>();
    }


}
