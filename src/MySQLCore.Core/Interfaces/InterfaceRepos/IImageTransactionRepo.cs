using System;
using MySQLCore.Core.Models.DTOs.ImageDTOs;

namespace MySQLCore.Core.Interfaces.InterfaceRepos;

public interface IImageTransactionRepo
{
    Task<List<ImageTransactionDTO>> GetAllRecords();
    Task<ImageTransactionDTO> GetRecordById(int id);
    Task<bool> CreateRecord(ImageTransactionDTO dto);
    Task<bool> UpdateRecord(ImageTransactionDTO dto);
    Task<bool> DeleteRecord(int id);
}
