namespace MySQLCore.Core.Enums;

public enum MessagerResultType
{
    NoAction,
    Success,
    NotFound,
    Conflict,
    Failed
}

public record ServiceResult( MessagerResultType Type, string Message );