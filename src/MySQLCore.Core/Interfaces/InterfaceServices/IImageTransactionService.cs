using MySQLCore.Core.Models.DTOs.ImageDTOs;

namespace MySQLCore.Core.Interfaces.InterfaceServices;

public interface IImageTransactionService
{
    Task<List<ImageTransactionDTO>> GetAllRecordsAsync();
    Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page);
    Task<ImageTransactionDTO> GetRecordByIdAsync(int id);
    Task<bool> CreateRecordAsync(CreateImageTransactionDTO dto);
    Task<bool> UpdateRecordAsync(UpdateImageTransactionDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
