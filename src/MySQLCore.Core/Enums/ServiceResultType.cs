namespace MySQLCore.Core.Enums;

public enum ServiceResultType
{
    Success,
    NotFound,
    Conflict,
    Failed
}

public record ServiceResult( ServiceResultType Type, string Message );