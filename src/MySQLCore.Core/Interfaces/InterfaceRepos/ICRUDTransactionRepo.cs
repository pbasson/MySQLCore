using MySQLCore.Core.Models.DTOs;

namespace MySQLCore.Core.Interfaces.InterfaceRepos;

public interface ICRUDTransactionRepo
{
    Task<List<CRUDTransactionDTO>> GetAllRecordsAsync();
    Task<List<CRUDTransactionDTO>> GetAllRecordsPaginationAsync(int page);
    Task<CRUDTransactionDTO> GetRecordByIdAsync(int id);
    Task<bool> CreateRecordAsync(CreateCRUDTransactionDTO dto);
    Task<bool> UpdateRecordAsync(UpdateCRUDTransactionDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}

