using MySQLCore.Core.Models.DTOs.ImageDTOs;

namespace MySQLCore.Core.Interfaces.InterfaceRepos;

public interface IImageTransactionRepo
{
    Task<List<ImageTransactionDTO>> GetAllRecordsAsync();
    Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page);
    Task<ImageTransactionDTO> GetRecordByIdAsync(int id);
    Task<bool> CreateRecordAsync(ImageTransactionDTO dto);
    Task<bool> UpdateRecordAsync(ImageTransactionDTO dto);
    Task<bool> DeleteRecordByIdAsysc(int id);
}
