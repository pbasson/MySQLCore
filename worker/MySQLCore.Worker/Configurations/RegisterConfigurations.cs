using MySQLCore.Worker.Constants.Settings;

namespace MySQLCore.Worker.Configurations;

public static class RegisterConfigurations
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        RegisterSeq();
        RegisterOpenTelemetry(services);
        RegisterMetrics(services);
        RegisterMessager(services, configuration);
        services.RegisterDatabase(configuration);
        services.RegisterService();
        RegisterBackgroundServices(services);
        return services;
    }

    private static void RegisterMessager(IServiceCollection services, IConfiguration configuration)
    {
        switch (configuration["Messaging:Provider"])
        {
            case "RabbitMQ":
                services.Configure<RabbitMQSetting>(configuration.GetSection("RabbitMQ"));
                services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
                services.AddSingleton<IMessageBus, RabbitMQBus>();
                services.AddHostedService<RabbitMQImageWorker>();
                break;

            case "AzureServiceBus":
                services.AddOptions<AzureServiceBusSetting>()
                    .Bind(configuration.GetSection("Messaging:AzureServiceBus"))
                    .Validate(settings => !string.IsNullOrWhiteSpace(settings.FullyQualifiedNamespace),
                        "Messaging:AzureServiceBus:FullyQualifiedNamespace is required.")
                    .Validate(settings => Uri.CheckHostName(settings.FullyQualifiedNamespace) == UriHostNameType.Dns,
                        "Azure Service Bus namespace must be a hostname without a scheme or path.")
                    .ValidateOnStart();

                services.AddSingleton<IMessageBus, AzureServiceBus>();
                services.AddHostedService<AzureServiceBusImageWorker>();

                services.AddSingleton<ServiceBusClient>(provider =>
                {
                    var settings = provider.GetRequiredService<IOptions<AzureServiceBusSetting>>().Value;
                    return new ServiceBusClient(settings.FullyQualifiedNamespace, new DefaultAzureCredential());
                });

                break;

            default:
                throw new InvalidOperationException("Messaging:Provider is Required.");
        }
    }

    private static void RegisterBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<OutboxWorker>();
    }

    private static void RegisterSeq()
    {
        string seqUrl = Environment.GetEnvironmentVariable(AppSettings.SEQ_URL) ?? "http://seq";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", "mysqlcore-worker")
            .WriteTo.Console()
            .WriteTo.Seq(seqUrl)
            .CreateLogger();
    }

    private static void RegisterOpenTelemetry(IServiceCollection services)
    {
        string otelCollectorURL = Environment.GetEnvironmentVariable(AppSettings.OTEL_EXPORTER_OTLP_ENDPOINT) ?? "http://otel-collector:4317";

        services.AddOpenTelemetry().ConfigureResource(resource => resource.AddService(serviceName: "mysqlcore-worker"))
            .WithTracing(tracing => tracing
                .SetSampler(new AlwaysOnSampler())
                .AddSource(TracingConstants.ACTIVITY_SOURCE)
                .AddSource(TracingConstants.API_SOURCE)
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otelCollectorURL);
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }));
    }

    private static void RegisterMetrics(IServiceCollection services)
    {
        services.AddMetricServer(options => { options.Port = 9100; });
    }

    public static HostApplicationBuilder RegisterHost(this HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);
        return builder;
    }
}
