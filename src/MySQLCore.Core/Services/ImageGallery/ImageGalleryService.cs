namespace MySQLCore.Core.Services.ImageGallery;

public sealed class ImageGalleryService : BaseService, IImageGalleryService
{
    private readonly IImageGalleryRepo _repo = default!;

    public ImageGalleryService(ILogger<ImageGalleryService> logger, ICacheService cache, IImageGalleryRepo repo): base(logger, cache)
    {
        _repo = repo;
    }

    public async Task<TransferImageGalleryGridDTO> GetLatestRecordsAsync(CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(ImageGalleryService), nameof(GetLatestRecordsAsync));

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(loggingHolder.Function);

        var cacheKey = $"image:{loggingHolder.Function}";

        var cached = await _cache.GetAsync<List<ImageGalleryDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", loggingHolder.Class, loggingHolder.Function, cacheKey);
            return new TransferImageGalleryGridDTO(cached!); 
        }

        var result = await _repo.GetLatestRecordsAsync(cancellationToken);
        if (result == null || result.Count <= 0) 
        { 
            _logger.LogWarning("{class}.{function}: No records found", loggingHolder.Class, loggingHolder.Function);
            return new TransferImageGalleryGridDTO(); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageGalleryGridDTO( result);
    }

    public async Task<TransferImageGalleryGridDTO> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(ImageGalleryService), nameof(GetRecordsByPaginationAsync));

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(loggingHolder.Function);
        activity?.SetTag("page", page);

        var cacheKey = $"image:{loggingHolder.Function}:page={page}";
        
        var cached = await _cache.GetAsync<List<ImageGalleryDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", loggingHolder.Class, loggingHolder.Function, cacheKey);
            return new TransferImageGalleryGridDTO(cached!); 
        }

        var result = await _repo.GetRecordsByPaginationAsync(page, cancellationToken);
        if (result == null || result.Count <= 0)  
        { 
            _logger.LogWarning("{class}.{function}: No records found", loggingHolder.Class, loggingHolder.Function);
            return new TransferImageGalleryGridDTO(); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageGalleryGridDTO(result); 
    }

    public async Task<TransferImageGalleryDTO> GetRecordByIdAsync(int id, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(ImageGalleryService), nameof(GetRecordByIdAsync));

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(loggingHolder.Function);
        activity?.SetTag("id", id);

        var cacheKey = $"image:{loggingHolder.Function}:id={id}";
        
        var cached = await _cache.GetAsync<ImageGalleryDTO>(cacheKey);
        if (cached != null && cached.ImageGalleryId > 0) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", loggingHolder.Class, loggingHolder.Function, cacheKey);
            return new TransferImageGalleryDTO( cached); 
        }
        else if (cached != null) { await _cache.RemoveAsync(cacheKey); }

        var result = await _repo.GetRecordByIdAsync(id, cancellationToken);
        if (result == null || result.ImageGalleryId <= 0)
        {
            _logger.LogWarning("{class}.{function}: No record found for {Id}", loggingHolder.Class, loggingHolder.Function, id);
            return new TransferImageGalleryDTO();
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageGalleryDTO(result); 
    }

    public async Task<TransferImageGalleryGridDTO> GetRecordsByGalleryNameAsync(string galleryName, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(ImageGalleryService), nameof(GetRecordsByGalleryNameAsync));

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(nameof(GetRecordsByGalleryNameAsync));
        activity?.SetTag("page", galleryName);

        var cacheKey = $"image:{loggingHolder.Function}:page={galleryName}";
        
        var cached = await _cache.GetAsync<List<ImageGalleryDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", loggingHolder.Class, loggingHolder.Function, cacheKey);
            return new TransferImageGalleryGridDTO( cached!); 
        }

        var result = await _repo.GetRecordsByGalleryNameAsync(galleryName, cancellationToken);
        if (result == null || result.Count <= 0)  
        { 
            _logger.LogWarning("{class}.{function}: No records found", loggingHolder.Class, loggingHolder.Function);
            return new TransferImageGalleryGridDTO(); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new TransferImageGalleryGridDTO( result); 
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageGalleryDTO dto, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(ImageGalleryService), nameof(CreateRecordAsync));

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(loggingHolder.Function);
        activity?.SetTag("dto.type", nameof(CreateImageGalleryDTO));

        var result = await _repo.CreateRecordAsync(dto, cancellationToken);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log}", loggingHolder.Class, loggingHolder.Function, "EntityNotCreated");
            return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); 
        }
        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageGalleryDTO dto, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(ImageGalleryService), nameof(UpdateRecordAsync));

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(loggingHolder.Function);
        activity?.SetTag("dto.ImageGalleryId", dto.ImageGalleryId);
        activity?.SetTag("dto.type", nameof(UpdateImageGalleryDTO));

        var result = await _repo.UpdateRecordAsync(dto, cancellationToken);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log} for {Id}", loggingHolder.Class, loggingHolder.Function, "EntityNotUpdated", dto.ImageGalleryId);
            return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); 
        }

        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        await _cache.RemoveAsync($"image:GetRecordByIdAsync:id={dto.ImageGalleryId}");
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(ImageGalleryService), nameof(DeleteRecordByIdAsync));

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryService>(loggingHolder.Function);
        activity?.SetTag("id", id);

        var result = await _repo.DeleteRecordByIdAsync(id, cancellationToken);
        if (!result)
        {
            _logger.LogWarning("{class}.{function}: No record deleted for {Id}", loggingHolder.Class, loggingHolder.Function, id);
        }

        await _cache.RemoveAsync("image:GetAllRecordsAsync");
        await _cache.RemoveAsync($"image:GetRecordByIdAsync:id={id}");
        return result;
    }
}
