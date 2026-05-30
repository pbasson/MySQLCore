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
    public async Task<TransferCRUDTransactionGridDTO> GetAllRecordsAsync()
    {
        using Activity? activity = TracingConstants.StartApiActivity<CRUDTransactionService>(nameof(GetAllRecordsAsync));

        var cacheKey = $"crud:GetAllRecordsAsync";

        var cached = await _cache.GetAsync<List<CRUDTransactionDTO>>(cacheKey);
        if (cached != null) { return new TransferCRUDTransactionGridDTO(ActionStatusType.Ok, cached!); }

        var result = await _repo.GetAllRecordsAsync();
        if (result == null || result.Count <= 0)  { return new TransferCRUDTransactionGridDTO(ActionStatusType.NotFound, []); }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferCRUDTransactionGridDTO(ActionStatusType.Ok, result); 

    }

    public async Task<TransferCRUDTransactionGridDTO> GetAllRecordsPaginationAsync(int page)
    {
        using Activity? activity = TracingConstants.StartApiActivity<CRUDTransactionService>(nameof(GetAllRecordsPaginationAsync));
        activity?.SetTag("page", page);

        var cacheKey = $"crud:GetAllRecordsPaginationAsync:page={page}";
        
        var cached = await _cache.GetAsync<List<CRUDTransactionDTO>>(cacheKey);
        if (cached != null) { return new TransferCRUDTransactionGridDTO(ActionStatusType.Ok, cached!); }

        var result = await _repo.GetAllRecordsPaginationAsync(page);
        if (result == null || result.Count <= 0)  { return new TransferCRUDTransactionGridDTO(ActionStatusType.NotFound, []); }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferCRUDTransactionGridDTO(ActionStatusType.Ok, result); 

    }

    public async Task<TransferCRUDTransactionDTO> GetRecordByIdAsync(int id)
    {
        using Activity? activity = TracingConstants.StartApiActivity<CRUDTransactionService>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var cacheKey = $"crud:GetRecordByIdAsync:id={id}";
        
        var cached = await _cache.GetAsync<CRUDTransactionDTO>(cacheKey);
        if (cached != null && cached.Id > 0) { return new TransferCRUDTransactionDTO(ActionStatusType.Ok, cached); }
        else if (cached != null) { await _cache.RemoveAsync(cacheKey); }

        var result = await _repo.GetRecordByIdAsync(id);
        if (result == null || result.Id <= 0) { return new TransferCRUDTransactionDTO(ActionStatusType.NotFound, new()); }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferCRUDTransactionDTO(ActionStatusType.Ok, result); 
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateCRUDTransactionDTO dto)
    {
        using Activity? activity = TracingConstants.StartApiActivity<CRUDTransactionService>(nameof(CreateRecordAsync));
        activity?.SetTag("dto.type", nameof(CreateCRUDTransactionDTO));

        var result = await _repo.CreateRecordAsync(dto);
        if (result == null || !result.Success) { return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); }
        await _cache.RemoveAsync("crud:GetAllRecordsAsync");
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateCRUDTransactionDTO dto)
    {
        using Activity? activity = TracingConstants.StartApiActivity<CRUDTransactionService>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto.Id", dto.Id);
        activity?.SetTag("dto.type", nameof(UpdateCRUDTransactionDTO));

        var result = await _repo.UpdateRecordAsync(dto);
        if (result == null || !result.Success) { return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); }

        await _cache.RemoveAsync("crud:GetAllRecordsAsync");
        await _cache.RemoveAsync($"crud:GetRecordByIdAsync:id={dto.Id}");
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        using Activity? activity = TracingConstants.StartApiActivity<CRUDTransactionService>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _repo.DeleteRecordByIdAsync(id);
        await _cache.RemoveAsync("crud:GetAllRecordsAsync");
        await _cache.RemoveAsync($"crud:GetRecordByIdAsync:id={id}");
        return result;
    }
 
}
