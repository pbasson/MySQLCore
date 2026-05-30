namespace MySQLCore.Core.Interfaces.Services;

public interface IImageTransactionService
{
    Task<TransferImageTransactionGridDTO> GetAllRecordsAsync();
    Task<TransferImageTransactionGridDTO> GetAllRecordsPaginationAsync(int page);
    Task<TransferImageTransactionDTO> GetRecordByIdAsync(int id);
    Task<TransferDTO> CreateRecordAsync(CreateImageTransactionDTO dto);
    Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
