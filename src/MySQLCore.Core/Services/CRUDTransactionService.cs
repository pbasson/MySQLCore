using MySQLCore.Core.DTOs;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Interfaces.InterfaceServices;

namespace MySQLCore.Core.Services;

public class CRUDTransactionService : ICRUDTransactionService
{
    private readonly ICRUDTransactionRepo _repo = default!;

    public CRUDTransactionService(ICRUDTransactionRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<CRUDTransactionDTO>> GetAllRecords()
    {
        try
        {
            var result = await _repo.GetAllRecords();
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<CRUDTransactionDTO> GetRecordById(int id)
    {
        try
        {
            var result = await _repo.GetRecordById(id);
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CreateRecord(CRUDTransactionDTO dto)
    {
        try
        {
            var result = await _repo.CreateRecord(dto);
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }


    public async Task<bool> UpdateRecord(CRUDTransactionDTO dto)
    {
        try
        {
            var result = await _repo.UpdateRecord(dto);
            return result;
        }
        catch (Exception)
        {
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
        catch (Exception)
        {
            throw;
        }
    }
}
