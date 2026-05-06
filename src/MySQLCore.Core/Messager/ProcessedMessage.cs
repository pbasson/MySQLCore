namespace MySQLCore.Core.Messager;

public class ProcessedMessage
{
    public int Id { get; set; }
    public Guid MessageId { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    public ProcessedMessage(Guid messageId)
    {
        MessageId = messageId;
        ProcessedAt = DateTime.UtcNow;
    }
}