using MySQLCore.Core.Models.DTOs.ImageDTOs;

namespace MySQLCore.Core.Interfaces.Repos;

public interface IImageTransactionRepo
{
    Task<List<ImageTransactionDTO>> GetAllRecordsAsync();
    Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page);
    Task<ImageTransactionDTO> GetRecordByIdAsync(int id);
    Task<TransferDTO> CreateRecordAsync(CreateImageTransactionDTO dto);
    Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
