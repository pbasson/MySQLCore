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

    public async Task<List<ImageTransactionDTO>> GetAllRecordsAsync()
    {
        try
        {
            var result = await _repo.GetAllRecordsAsync();
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page)
    {
        try
        {
            var result = await _repo.GetAllRecordsPaginationAsync(page);
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }


    public async Task<ImageTransactionDTO> GetRecordByIdAsync(int id)
    {
        try
        {
            var result = await _repo.GetRecordByIdAsync(id);
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }

    public async Task<bool> CreateRecordAsync(ImageTransactionDTO dto)
    {
        try
        {
            var result = await _repo.CreateRecordAsync(dto);
            return result;
        }
       catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }


    public async Task<bool> UpdateRecordAsync(ImageTransactionDTO dto)
    {
        try
        {
            var result = await _repo.UpdateRecordAsync(dto);
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        try
        {
            var result = await _repo.DeleteRecordByIdAsysc(id);
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }
}
