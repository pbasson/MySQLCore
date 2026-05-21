namespace MySQLCore.API.Configurations;

public static class RegisterConfigurations
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        RegisterSeq();
        RegisterOpenTelemetry(services);
        RegisterAPIConfigure(services);
        RegisterMessager(services, configuration);

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

    private static void RegisterAPIConfigure(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddEndpointsApiExplorer();
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
        services.AddHostedService<OutboxPublisherWorker>();
        services.AddHostedService<ImageProcessingWorker>();
    }

    private static void RegisterSeq()
    {
        string seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://seq";
        string logPath = Environment.GetEnvironmentVariable("LOG_PATH") ?? "/Logs/mysqlcore-log-.txt";

        Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            .Filter.ByExcluding(logEvent =>
                logEvent.RenderMessage().Contains("/metrics") || logEvent.RenderMessage().Contains("Prometheus metrics"))
            .WriteTo.Console().WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .WriteTo.Seq(serverUrl: seqUrl)
            .CreateLogger();
    }

    private static void RegisterOpenTelemetry(IServiceCollection services)
    {
        string tempoUrl = Environment.GetEnvironmentVariable("TEMPO_URL") ?? "http://tempo:4318";
        string otelCollectorURL = "http://otel-collector:4317";
        services.AddOpenTelemetry().ConfigureResource(resource => resource.AddService(serviceName: TracingConstants.SERVICE_NAME))
            .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation()
                .SetSampler(new AlwaysOnSampler())
                .AddHttpClientInstrumentation().AddSource(TracingConstants.ACTIVITY_SOURCE)
                .AddHttpClientInstrumentation().AddSource(TracingConstants.REPO_SOURCE)
                // .AddConsoleExporter()
                .AddOtlpExporter(options => { options.Endpoint = new Uri(otelCollectorURL);
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                 }));
    }
    
    private static void RegisterMessager(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
    }

}
