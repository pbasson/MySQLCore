namespace MySQLCore.Core.Interfaces.Services;

public interface IUserService
{
    Task<UserTransferGridDTO> GetAllRecordsAsync();
    Task<UserTransferGridDTO> GetRecordsByPaginationAsync(int page);
    Task<UserTransferDTO> GetRecordByIdAsync(int id);
    Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto);
    Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
