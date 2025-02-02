using MySQLCore.Core.Models.DTOs;
using MySQLCore.Core.Models.DTOs.ImageDTOs;

namespace MySQLCore.Core.Interfaces.InterfaceServices;

public interface IImageTransactionService
{
    Task<List<ImageTransactionDTO>> GetAllRecords();
    Task<ImageTransactionDTO> GetRecordById(int id);
    Task<bool> CreateRecord(ImageTransactionDTO dto);
    Task<bool> UpdateRecord(ImageTransactionDTO dto);
    Task<bool> DeleteRecord(int id);
}
