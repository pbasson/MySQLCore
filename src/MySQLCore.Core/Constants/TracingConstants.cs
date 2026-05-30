namespace MySQLCore.Core.Constants;

public static class TracingConstants
{
    public const string SERVICE_NAME = "mysqlcore-api";
    public const string ACTIVITY_SOURCE = "MySQLCore.Messaging";
    public const string API_SOURCE = "MySQLCore.API";

    public static readonly ActivitySource MessagingActivitySource = new(ACTIVITY_SOURCE);
    public static readonly ActivitySource APIActivitySource = new(API_SOURCE);

    public static Activity? StartApiActivity<T>(string methodName)
    {
        return APIActivitySource.StartActivity($"{typeof(T).Name}.{methodName}");
    }

    public static Activity? StartMessagingActivity<T>(string methodName)
    {
        return MessagingActivitySource.StartActivity($"{typeof(T).Name}.{methodName}");
    }
}
