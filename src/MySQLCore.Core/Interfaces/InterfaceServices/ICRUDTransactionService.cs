using MySQLCore.Core.Models.DTOs;

namespace MySQLCore.Core.Interfaces.InterfaceServices;

public interface ICRUDTransactionService
{
    Task<List<CRUDTransactionDTO>> GetAllRecords();
    Task<CRUDTransactionDTO> GetRecordById(int id);
    Task<bool> CreateRecord(CRUDTransactionDTO dto);
    Task<bool> UpdateRecord(CRUDTransactionDTO dto);
    Task<bool> DeleteRecord(int id);
}
