namespace MySQLCore.Core.Services;

public class CRUDTransactionService : BaseService, ICRUDTransactionService 
{
    private readonly ICRUDTransactionRepo _repo = default!;

    public CRUDTransactionService(ILogger<CRUDTransactionService> logger, ICacheService cache,ICRUDTransactionRepo repo): base(logger, cache)
    {
        _repo = repo;
    }

    /// <summary>
    /// Get All Records 
    /// </summary>
    public async Task<List<CRUDTransactionDTO>> GetAllRecordsAsync()
    {
        var cacheKey = $"crud:GetAllRecordsAsync";

        var cached = await _cache.GetAsync<List<CRUDTransactionDTO>>(cacheKey);
        if (cached != null) { return cached; }

        var result = await _repo.GetAllRecordsAsync();
        if (result == null || result.Count <= 0) { return []; }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return result;
    }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsPaginationAsync(int page)
    {
        var cacheKey = $"crud:GetAllRecordsPaginationAsync:page={page}";
        
        var cached = await _cache.GetAsync<List<CRUDTransactionDTO>>(cacheKey);
        if (cached != null) { return cached; }

        var result = await _repo.GetAllRecordsPaginationAsync(page);
        if (result == null || result.Count <= 0) { return []; }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return result;
    }

    public async Task<CRUDTransactionDTO> GetRecordByIdAsync(int id)
    {
        var cacheKey = $"image:GetRecordByIdAsync:id={id}";
        
        var cached = await _cache.GetAsync<CRUDTransactionDTO>(cacheKey);
        if (cached != null) { return cached; }

        var result = await _repo.GetRecordByIdAsync(id);

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return result;
    }

    public async Task<bool> CreateRecordAsync(CreateCRUDTransactionDTO dto)
    {
        var result = await _repo.CreateRecordAsync(dto);
        await _cache.RemoveAsync("crud:GetAllRecordsAsync");
        return result;
    }

    public async Task<bool> UpdateRecordAsync(UpdateCRUDTransactionDTO dto)
    {
        var result = await _repo.UpdateRecordAsync(dto);
        await _cache.RemoveAsync("crud:GetAllRecordsAsync");
        await _cache.RemoveAsync($"crud:GetRecordByIdAsync:id={dto.Id}");
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        var result = await _repo.DeleteRecordByIdAsync(id);
        await _cache.RemoveAsync($"crud:GetRecordByIdAsync:id={id}");
        return result;
    }
 
}
