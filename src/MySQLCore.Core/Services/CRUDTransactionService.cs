namespace MySQLCore.Core.Services;

public class CRUDTransactionService(ICRUDTransactionRepo repo) : ICRUDTransactionService
{
    private readonly ICRUDTransactionRepo _repo = repo;

    /// <summary>
    /// Get All Records 
    /// </summary>
    public async Task<List<CRUDTransactionDTO>> GetAllRecordsAsync()
    {
        var result = await _repo.GetAllRecordsAsync();
        return result;
    }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsPaginationAsync(int page)
    {
        var result = await _repo.GetAllRecordsPaginationAsync(page);
        return result;
    }

    public async Task<CRUDTransactionDTO> GetRecordByIdAsync(int id)
    {
        var result = await _repo.GetRecordByIdAsync(id);
        return result;
    }

    public async Task<bool> CreateRecordAsync(CreateCRUDTransactionDTO dto)
    {
        var result = await _repo.CreateRecordAsync(dto);
        return result;
    }


    public async Task<bool> UpdateRecordAsync(UpdateCRUDTransactionDTO dto)
    {
        var result = await _repo.UpdateRecordAsync(dto);
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        var result = await _repo.DeleteRecordByIdAsync(id);
        return result;
    }
 
}
