namespace MySQLCore.Core.Interfaces.Repos.User;

public interface IUserRepo
{
    Task<List<UserDTO>> GetAllRecordsAsync();
    Task<List<UserDTO>> GetRecordsByPaginationAsync(int page);
    Task<UserDTO?> GetRecordByIdAsync(int id);
    Task<UserDTO?> GetUsernameAsync(string username);
    Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto);
    Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
