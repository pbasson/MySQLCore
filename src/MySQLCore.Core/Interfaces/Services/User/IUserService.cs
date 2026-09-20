namespace MySQLCore.Core.Interfaces.Services.User;

public interface IUserService
{
    Task<UserTransferGridDTO> GetLatestRecordsAsync(CancellationToken cancellationToken);
    Task<UserTransferGridDTO> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken);
    Task<UserTransferDTO> GetRecordByIdAsync(int id, CancellationToken cancellationToken);
    Task<UserTransferDTO> GetUsernameAsync(string username, CancellationToken cancellationToken);
    Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto, CancellationToken cancellationToken);
    Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto, CancellationToken cancellationToken);
    Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken);
}
