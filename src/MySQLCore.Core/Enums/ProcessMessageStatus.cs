public enum ProcessMessageStatus
{
    Pending = 1,
    Processing = 2,
    Processed = 3,
    Acknowledged = 4,
    Failed = 5,
    Invalid = 6,
    DeadLetter = 7,
    IgnoredDuplicate = 8
}