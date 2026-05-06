namespace MySQLCore.Core.Interfaces.Messager;

public interface IProcessedMessageRepo
{
    Task AddAsync(Guid messageId);
    Task<bool> ExistsAsync(Guid messageId);
}