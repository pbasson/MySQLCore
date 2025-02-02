using ElmahCore;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Interfaces.InterfaceServices;
using MySQLCore.Core.Models.DTOs.ImageDTOs;

namespace MySQLCore.Core.Services;

public class ImageTransactionService : IImageTransactionService
{
    private readonly IImageTransactionRepo _repo = default!;

    public ImageTransactionService(IImageTransactionRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecords()
    {
        try
        {
            var result = await _repo.GetAllRecords();
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }

    public async Task<ImageTransactionDTO> GetRecordById(int id)
    {
        try
        {
            var result = await _repo.GetRecordById(id);
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }

    public async Task<bool> CreateRecord(ImageTransactionDTO dto)
    {
        try
        {
            var result = await _repo.CreateRecord(dto);
            return result;
        }
       catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }


    public async Task<bool> UpdateRecord(ImageTransactionDTO dto)
    {
        try
        {
            var result = await _repo.UpdateRecord(dto);
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }

    public async Task<bool> DeleteRecord(int id)
    {
        try
        {
            var result = await _repo.DeleteRecord(id);
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }
}
