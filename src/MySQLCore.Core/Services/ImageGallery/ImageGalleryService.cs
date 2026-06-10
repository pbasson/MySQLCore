namespace MySQLCore.Core.Services.ImageGallery;

public class ImageGalleryService : BaseService, IImageGalleryService
{
    private readonly IImageGalleryRepo _repo = default!;

    public ImageGalleryService(ILogger<ImageGalleryService> logger, ICacheService cache, IImageGalleryRepo repo): base(logger, cache)
    {
        _repo = repo;
    }

    public async Task<TransferImageGalleryGridDTO> GetAllRecordsAsync()
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(nameof(GetAllRecordsAsync));

        var cacheKey = $"image:GetAllRecordsAsync";

        var cached = await _cache.GetAsync<List<ImageGalleryDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(ImageGalleryService), nameof(GetAllRecordsAsync), cacheKey);
            return new TransferImageGalleryGridDTO(ActionStatusType.Ok, cached!); 
        }

        var result = await _repo.GetAllRecordsAsync();
        if (result == null || result.Count <= 0) 
        { 
            _logger.LogWarning("{class}.{function}: No records found", nameof(ImageGalleryService), nameof(GetAllRecordsAsync));
            return new TransferImageGalleryGridDTO(ActionStatusType.NotFound); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageGalleryGridDTO(ActionStatusType.Ok, result);
    }


    public async Task<TransferImageGalleryGridDTO> GetRecordsByPaginationAsync(int page)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(nameof(GetRecordsByPaginationAsync));
        activity?.SetTag("page", page);

        var cacheKey = $"image:GetAllRecordsPaginationAsync:page={page}";
        
        var cached = await _cache.GetAsync<List<ImageGalleryDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(ImageGalleryService), nameof(GetRecordsByPaginationAsync), cacheKey);
            return new TransferImageGalleryGridDTO(ActionStatusType.Ok, cached!); 
        }

        var result = await _repo.GetRecordsByPaginationAsync(page);
        if (result == null || result.Count <= 0)  
        { 
            _logger.LogWarning("{class}.{function}: No records found", nameof(ImageGalleryService), nameof(GetRecordsByPaginationAsync));
            return new TransferImageGalleryGridDTO(ActionStatusType.NotFound); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageGalleryGridDTO(ActionStatusType.Ok, result); 
    }

    public async Task<TransferImageGalleryDTO> GetRecordByIdAsync(int id)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var cacheKey = $"image:GetRecordByIdAsync:id={id}";
        
        var cached = await _cache.GetAsync<ImageGalleryDTO>(cacheKey);
        if (cached != null && cached.ImageGalleryId > 0) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(ImageGalleryService), nameof(GetRecordByIdAsync), cacheKey);
            return new TransferImageGalleryDTO(ActionStatusType.Ok, cached); 
        }
        else if (cached != null) { await _cache.RemoveAsync(cacheKey); }

        var result = await _repo.GetRecordByIdAsync(id);
        if (result == null || result.ImageGalleryId <= 0)
        {
            _logger.LogWarning("{class}.{function}: No record found for {Id}", nameof(ImageGalleryService), nameof(GetRecordByIdAsync), id);
            return new TransferImageGalleryDTO(ActionStatusType.NotFound);
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageGalleryDTO(ActionStatusType.Ok, result); 
    }


    public async Task<TransferImageGalleryGridDTO> GetRecordsByGalleryNameAsync(string galleryName)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(nameof(GetRecordsByGalleryNameAsync));
        activity?.SetTag("page", galleryName);

        var cacheKey = $"image:GetRecordsByGalleryNameAsync:page={galleryName}";
        
        var cached = await _cache.GetAsync<List<ImageGalleryDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(ImageGalleryService), nameof(GetRecordsByGalleryNameAsync), cacheKey);
            return new TransferImageGalleryGridDTO(ActionStatusType.Ok, cached!); 
        }

        var result = await _repo.GetRecordsByGalleryNameAsync(galleryName);
        if (result == null || result.Count <= 0)  
        { 
            _logger.LogWarning("{class}.{function}: No records found", nameof(ImageGalleryService), nameof(GetRecordsByGalleryNameAsync));
            return new TransferImageGalleryGridDTO(ActionStatusType.NotFound); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageGalleryGridDTO(ActionStatusType.Ok, result); 
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageGalleryDTO dto)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(nameof(CreateRecordAsync));
        activity?.SetTag("dto.type", nameof(CreateImageGalleryDTO));

        var result = await _repo.CreateRecordAsync(dto);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log}", nameof(ImageGalleryService), nameof(CreateRecordAsync), "EntityNotCreated");
            return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); 
        }
        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageGalleryDTO dto)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto.ImageGalleryId", dto.ImageGalleryId);
        activity?.SetTag("dto.type", nameof(UpdateImageGalleryDTO));

        var result = await _repo.UpdateRecordAsync(dto);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log} for {Id}", nameof(ImageGalleryService), nameof(UpdateRecordAsync), "EntityNotUpdated", dto.ImageGalleryId);
            return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); 
        }

        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        await _cache.RemoveAsync($"image:GetRecordByIdAsync:id={dto.ImageGalleryId}");
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _repo.DeleteRecordByIdAsync(id);
        if (!result)
        {
            _logger.LogWarning("{class}.{function}: No record deleted for {Id}", nameof(ImageGalleryService), nameof(DeleteRecordByIdAsync), id);
        }

        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        await _cache.RemoveAsync($"image:GetRecordByIdAsync:id={id}");
        return result;
    }
}
