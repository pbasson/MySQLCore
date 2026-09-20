namespace MySQLCore.Core.Enums;

public enum OutboxMessageStatus
{
    Pending = 1,
    Processing = 2,
    Published = 3,
    Failed = 4,
    DeadLetter = 5
}