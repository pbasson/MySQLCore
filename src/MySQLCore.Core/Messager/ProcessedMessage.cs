namespace MySQLCore.Core.Messager;

public class ProcessedMessage
{
    public int Id { get; set; }
    public Guid MessageId { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public ProcessMessageStatus Status { get; set; } 
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

public class ProcessedMessageTransfer
{
    public ProcessedMessage GetTransfer(Guid messageId, string messageType, string entityName, int entityId)
    {
        return new ProcessedMessage()
        {
            MessageId = messageId,
            MessageType = messageType,
            EntityName = entityName,
            EntityId = entityId,
            Status = ProcessMessageStatus.Processing,
            ProcessedAt = DateTime.UtcNow,
        };
    }
}