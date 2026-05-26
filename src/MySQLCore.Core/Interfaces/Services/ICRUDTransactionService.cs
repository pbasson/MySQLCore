namespace MySQLCore.Core.Interfaces.Services;

public interface ICRUDTransactionService
{
    Task<List<CRUDTransactionDTO>> GetAllRecordsAsync();
    Task<List<CRUDTransactionDTO>> GetAllRecordsPaginationAsync(int page);
    Task<CRUDTransactionDTO> GetRecordByIdAsync(int id);
    Task<TransferDTO> CreateRecordAsync(CreateCRUDTransactionDTO dto);
    Task<TransferDTO> UpdateRecordAsync(UpdateCRUDTransactionDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
