namespace MySQLCore.Core.Services;

public class ImageTransactionService : BaseService, IImageTransactionService
{
    private readonly IImageTransactionRepo _repo = default!;

    public ImageTransactionService(ILogger<ImageTransactionService> logger, ICacheService cache, IImageTransactionRepo repo): base(logger, cache)
    {
        _repo = repo;
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsAsync()
    {
        var cacheKey = $"image:GetAllRecordsAsync";

        var cached = await _cache.GetAsync<List<ImageTransactionDTO>>(cacheKey);
        if (cached != null) { return cached; }

        var result = await _repo.GetAllRecordsAsync();
        if (result == null || result.Count <= 0) { return []; }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return result;
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page)
    {
        var cacheKey = $"image:GetAllRecordsPaginationAsync:page={page}";
        
        var cached = await _cache.GetAsync<List<ImageTransactionDTO>>(cacheKey);
        if (cached != null) { return cached; }

        var result = await _repo.GetAllRecordsPaginationAsync(page);
        if (result == null || result.Count <= 0) { return []; }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return result;
    }

    public async Task<ImageTransactionDTO> GetRecordByIdAsync(int id)
    {
        var cacheKey = $"image:GetRecordByIdAsync:id={id}";
        
        var cached = await _cache.GetAsync<ImageTransactionDTO>(cacheKey);
        if (cached != null) { return cached; }

        var result = await _repo.GetRecordByIdAsync(id);

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return result;
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageTransactionDTO dto)
    {
        var result = await _repo.CreateRecordAsync(dto);
        await _cache.RemoveAsync("images:GetAllRecordsAsync");
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto)
    {
        var result = await _repo.UpdateRecordAsync(dto);
        await _cache.RemoveAsync("images:GetAllRecordsAsync");
        await _cache.RemoveAsync($"image:GetRecordByIdAsync:id={dto.ImageTransactionID}");
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        var result = await _repo.DeleteRecordByIdAsync(id);
        await _cache.RemoveAsync($"image:GetRecordByIdAsync:id={id}");
        return result;
    }
}
