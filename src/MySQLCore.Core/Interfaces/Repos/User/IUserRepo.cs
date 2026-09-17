namespace MySQLCore.Core.Interfaces.Repos.User;

public interface IUserRepo
{
    /// <summary>
    /// Get All Records
    /// </summary>
    /// <returns></returns>
    Task<List<UserDTO>> GetAllRecordsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get records by pagination
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns> 
    Task<List<UserDTO>> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken);

    /// <summary>
    /// Get latest records
    /// </summary>
    /// <returns></returns>
    Task<List<UserDTO>> GetLatestRecordsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get record by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<UserDTO?> GetRecordByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Get record by username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<UserDTO?> GetUsernameAsync(string username, CancellationToken cancellationToken);
    
    Task<bool> CheckEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Create a new User record
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto, CancellationToken cancellationToken);

    /// <summary>
    /// Update record
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto, CancellationToken cancellationToken);

    /// <summary>
    /// Delete record by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken);
}
