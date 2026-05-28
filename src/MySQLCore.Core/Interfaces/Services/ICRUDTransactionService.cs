namespace MySQLCore.Core.Interfaces.Services;

public interface ICRUDTransactionService
{
    Task<TransferCRUDTransactionGridDTO> GetAllRecordsAsync();
    Task<TransferCRUDTransactionGridDTO> GetAllRecordsPaginationAsync(int page);
    Task<TransferCRUDTransactionDTO> GetRecordByIdAsync(int id);
    Task<TransferDTO> CreateRecordAsync(CreateCRUDTransactionDTO dto);
    Task<TransferDTO> UpdateRecordAsync(UpdateCRUDTransactionDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
