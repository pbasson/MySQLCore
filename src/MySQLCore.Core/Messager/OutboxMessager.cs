using System.Text.Json;

namespace MySQLCore.Core.Messager;

public class OutboxMessage
{
    public long Id { get; set; }

    public Guid MessageId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public string? EntityName { get; set; }

    public int? EntityId { get; set; }

    public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Pending;

    public int RetryCount { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAt { get; set; }

    public DateTime? LastAttemptAt { get; set; }
}

public static class OutboxMessageTransfer
{
    public static OutboxMessage GetTransfer<TMessage>( Guid messageId, string eventType, TMessage message) where TMessage : IMessage
    {
        return new OutboxMessage
        {
            MessageId = messageId,
            EventType = eventType,
            Payload = JsonSerializer.Serialize(message),
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow
        };
    }
}