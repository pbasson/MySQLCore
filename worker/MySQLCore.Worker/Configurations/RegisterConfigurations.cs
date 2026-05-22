namespace MySQLCore.Worker.Configurations;

public static class RegisterConfigurations
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        RegisterSeq();
        RegisterMessager(services, configuration);
        services.RegisterDatabase(configuration);
        services.RegisterService();
        RegisterBackgroundServices(services);
        return services;
    }

    private static void RegisterMessager(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
    }

    private static void RegisterBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<OutboxPublisherWorker>();
        services.AddHostedService<ImageProcessingWorker>();
    }

    private static void RegisterSeq()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.Seq("http://seq")
            .CreateLogger();
    }

    //     private static void RegisterSeq( )
    // {
    //     string seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://seq";
    //     string logPath = Environment.GetEnvironmentVariable("LOG_PATH") ?? "/Logs/mysqlcore.worker-log-.txt";

    //     Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
    //         .Filter.ByExcluding(logEvent =>
    //             logEvent.RenderMessage().Contains("/metrics") || logEvent.RenderMessage().Contains("Prometheus metrics"))
    //         .WriteTo.Console().WriteTo.File(
    //             path: logPath,
    //             rollingInterval: RollingInterval.Day,
    //             retainedFileCountLimit: 7)
    //         .WriteTo.Seq(serverUrl: seqUrl)
    //         .CreateLogger();
    // }

    public static HostApplicationBuilder RegisterHost(this HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);
        return builder;
    }
}
