using ElmahCore;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Interfaces.InterfaceServices;
using MySQLCore.Core.Models.DTOs;

namespace MySQLCore.Core.Services;

public class CRUDTransactionService : ICRUDTransactionService
{
    private readonly ICRUDTransactionRepo _repo = default!;

    public CRUDTransactionService(ICRUDTransactionRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsAsync()
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

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsPaginationAsync(int page)
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

    public async Task<CRUDTransactionDTO> GetRecordByIdAsync(int id)
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

    public async Task<bool> CreateRecordAsync(CreateCRUDTransactionDTO dto)
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


    public async Task<bool> UpdateRecordAsync(UpdateCRUDTransactionDTO dto)
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
            var result = await _repo.DeleteRecordByIdAsync(id);
            return result;
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;
        }
    }

 
}
