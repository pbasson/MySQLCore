namespace MySQLCore.Core.Constants;


public class RabbitMQSettings
{
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string DeadLetterQueueName { get; set; } = string.Empty;

    public int MaxRetryCount { get; set; } = 3;
    public string RetryHeader = "x-retry-count";
}