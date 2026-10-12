namespace MySQLCore.Worker.Constants.Settings;

public sealed class AzureServiceBusSetting
{
    public string FullyQualifiedNamespace { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
}
