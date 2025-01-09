using MySQLCore.Core.DTOs;

namespace MySQLCore.Core.Interfaces.InterfaceRepos;

public interface ICRUDTransactionRepo
{
    Task<List<CRUDTransactionDTO>> GetAllRecords();
    Task<CRUDTransactionDTO> GetRecordById(int id);
    Task<bool> CreateRecord(CRUDTransactionDTO dto);
    Task<bool> UpdateRecord(CRUDTransactionDTO dto);
    Task<bool> DeleteRecord(int id);
}

