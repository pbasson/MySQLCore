namespace MySQLCore.Core.Interfaces.Repos.User;

public interface IUserRepo
{
    /// <summary>
    /// Get All Records
    /// </summary>
    /// <returns></returns>
    Task<List<UserDTO>> GetAllRecordsAsync();

    /// <summary>
    /// Get records by pagination
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns> 
    Task<List<UserDTO>> GetRecordsByPaginationAsync(int page);

    /// <summary>
    /// Get latest records
    /// </summary>
    /// <returns></returns>
    Task<List<UserDTO>> GetLatestRecordsAsync();

    /// <summary>
    /// Get record by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<UserDTO?> GetRecordByIdAsync(int id);

    /// <summary>
    /// Get record by username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<UserDTO?> GetUsernameAsync(string username);

    /// <summary>
    /// Create a new User record
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto);

    /// <summary>
    /// Update record
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto);

    /// <summary>
    /// Delete record by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteRecordByIdAsync(int id);
}
