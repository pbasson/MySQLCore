namespace MySQLCore.Core.Services;

public class ImageTransactionService : BaseService, IImageTransactionService
{
    private readonly IImageTransactionRepo _repo = default!;

    public ImageTransactionService(ILogger<ImageTransactionService> logger, ICacheService cache, IImageTransactionRepo repo): base(logger, cache)
    {
        _repo = repo;
    }

    public async Task<TransferImageTransactionGridDTO> GetAllRecordsAsync()
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionService>(nameof(GetAllRecordsAsync));

        var cacheKey = $"image:GetAllRecordsAsync";

        var cached = await _cache.GetAsync<List<ImageTransactionDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(ImageTransactionService), nameof(GetAllRecordsAsync), cacheKey);
            return new TransferImageTransactionGridDTO(ActionStatusType.Ok, cached!); 
        }

        var result = await _repo.GetAllRecordsAsync();
        if (result == null || result.Count <= 0) 
        { 
            _logger.LogWarning("{class}.{function}: No records found", nameof(ImageTransactionService), nameof(GetAllRecordsAsync));
            return new TransferImageTransactionGridDTO(ActionStatusType.NotFound); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageTransactionGridDTO(ActionStatusType.Ok, result);
    }


    public async Task<TransferImageTransactionGridDTO> GetRecordsByPaginationAsync(int page)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionService>(nameof(GetRecordsByPaginationAsync));
        activity?.SetTag("page", page);

        var cacheKey = $"image:GetAllRecordsPaginationAsync:page={page}";
        
        var cached = await _cache.GetAsync<List<ImageTransactionDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(ImageTransactionService), nameof(GetRecordsByPaginationAsync), cacheKey);
            return new TransferImageTransactionGridDTO(ActionStatusType.Ok, cached!); 
        }

        var result = await _repo.GetRecordsByPaginationAsync(page);
        if (result == null || result.Count <= 0)  
        { 
            _logger.LogWarning("{class}.{function}: No records found", nameof(UserService), nameof(GetRecordsByPaginationAsync));
            return new TransferImageTransactionGridDTO(ActionStatusType.NotFound); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageTransactionGridDTO(ActionStatusType.Ok, result); 
    }

    public async Task<TransferImageTransactionDTO> GetRecordByIdAsync(int id)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionService>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var cacheKey = $"image:GetRecordByIdAsync:id={id}";
        
        var cached = await _cache.GetAsync<ImageTransactionDTO>(cacheKey);
        if (cached != null && cached.ImageTransactionID > 0) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(ImageTransactionService), nameof(GetRecordByIdAsync), cacheKey);
            return new TransferImageTransactionDTO(ActionStatusType.Ok, cached); 
        }
        else if (cached != null) { await _cache.RemoveAsync(cacheKey); }

        var result = await _repo.GetRecordByIdAsync(id);
        if (result == null || result.ImageTransactionID <= 0) { return new TransferImageTransactionDTO(ActionStatusType.NotFound); }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageTransactionDTO(ActionStatusType.Ok, result); 
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageTransactionDTO dto)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionService>(nameof(CreateRecordAsync));
        activity?.SetTag("dto.type", nameof(CreateImageTransactionDTO));

        var result = await _repo.CreateRecordAsync(dto);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log}", nameof(ImageTransactionService), nameof(CreateRecordAsync), "EntityNotCreated");
            return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); 
        }
        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionService>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto.ImageTransactionID", dto.ImageTransactionID);
        activity?.SetTag("dto.type", nameof(UpdateImageTransactionDTO));

        var result = await _repo.UpdateRecordAsync(dto);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log} for {Id}", nameof(ImageTransactionService), nameof(UpdateRecordAsync), "EntityNotCreated", dto.ImageTransactionID);
            return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); 
        }

        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        await _cache.RemoveAsync($"image:GetRecordByIdAsync:id={dto.ImageTransactionID}");
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionService>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _repo.DeleteRecordByIdAsync(id);
        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        await _cache.RemoveAsync($"image:GetRecordByIdAsync:id={id}");
        return result;
    }
}
