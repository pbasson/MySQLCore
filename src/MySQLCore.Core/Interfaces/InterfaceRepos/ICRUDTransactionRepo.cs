using MySQLCore.Core.Models.DTOs;

namespace MySQLCore.Core.Interfaces.InterfaceRepos;

public interface ICRUDTransactionRepo
{
    Task<List<CRUDTransactionDTO>> GetAllRecords();
    Task<CRUDTransactionDTO> GetRecordById(int id);
    Task<bool> CreateRecord(CreateCRUDTransactionDTO dto);
    Task<bool> UpdateRecord(UpdateCRUDTransactionDTO dto);
    Task<bool> DeleteRecord(int id);
}

