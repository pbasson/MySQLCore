namespace MySQLCore.Core.Enums;

public enum ProcessMessageStatus
{
    Pending = 1,
    Published = 2,
    Received = 3,
    Processing = 4,
    Processed = 5,
    Acknowledged = 6,
    Completed = 7,
    Failed = 8,
    Invalid = 9,
    DeadLetter = 10,
    IgnoredDuplicate = 11
}