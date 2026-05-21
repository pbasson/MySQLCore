namespace MySQLCore.Worker.MetricManager;

public static class MessageMetrics
{
    public static readonly Counter Published =
        Metrics.CreateCounter("messages_published_total", "Total messages published");

    public static readonly Counter Received =
        Metrics.CreateCounter("messages_received_total", "Total messages Received");

    public static readonly Counter Processing =
        Metrics.CreateCounter("messages_processing_total", "Total messages Processing");

    public static readonly Counter Processed =
        Metrics.CreateCounter("messages_processed_total", "Total messages processed");

    public static readonly Counter Acknowledged =
        Metrics.CreateCounter("messages_acknowledged_total", "Total messages acknowledged");

    public static readonly Counter Failed =
        Metrics.CreateCounter("messages_failed_total", "Total messages failed");

    public static readonly Counter DeadLetter =
        Metrics.CreateCounter("messages_deadletter_total", "Total messages moved to DLQ");

    public static readonly Counter Duplicate =
        Metrics.CreateCounter("messages_duplicate_total", "Total duplicate messages ignored");
}