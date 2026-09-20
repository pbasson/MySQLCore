namespace MySQLCore.Core.Interfaces.Repos.User;

public interface IUserRepo : IBaseRepo
{
    /// <summary>
    /// Get latest records
    /// </summary>
    Task<List<UserDTO>> GetLatestRecordsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get records by pagination
    /// </summary>
    /// <param name="page"></param>
    Task<List<UserDTO>> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken);

    /// <summary>
    /// Get record by id
    /// </summary>
    Task<UserDTO?> GetRecordByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Get record by username
    /// </summary>
    Task<UserDTO?> GetUsernameAsync(string username, CancellationToken cancellationToken);
    
    Task<bool> CheckEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Create a new User record
    /// </summary>
    Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto, CancellationToken cancellationToken);

    /// <summary>
    /// Update record
    /// </summary>
    Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto, CancellationToken cancellationToken);

    /// <summary>
    /// Delete record by id
    /// </summary>
    Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken);
}
