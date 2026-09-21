namespace MySQLCore.Core.Services.User;

public sealed class UserService : BaseService, IUserService 
{
    private readonly IValidator<CreateUserDTO> _createValidator = default!;
    private readonly IValidator<UpdateUserDTO> _updateValidator = default!;
    private readonly IUserRepo _repo = default!;
    public const string module = "user";

    public UserService(ILogger<UserService> logger, ICacheService cache,IUserRepo repo, IValidator<CreateUserDTO> createValidator, IValidator<UpdateUserDTO> updateValidator): base(logger, cache, module)
    {
        _repo = repo;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<UserTransferGridDTO> GetLatestRecordsAsync(CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(UserService), nameof(GetLatestRecordsAsync));
        
        using Activity? activity = TracingConstants.StartApiActivity<UserService>(loggingHolder.Function);

        var cacheKey = $"{CacheKey}:{loggingHolder.Function}";

        var cached = await _cache.GetAsync<List<UserDTO>>(cacheKey);
        if (cached != null) 
        { 
            LogWarningNoRecord(loggingHolder);
            return new UserTransferGridDTO(cached!); 
        }

        var result = await _repo.GetLatestRecordsAsync(cancellationToken);
        if (result == null || result.Count <= 0)  
        { 
            LogWarningNoRecord(loggingHolder);
            return new UserTransferGridDTO([]); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new UserTransferGridDTO(result); 
    }

    public async Task<UserTransferGridDTO> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(UserService), nameof(GetRecordsByPaginationAsync));

        using Activity? activity = TracingConstants.StartApiActivity<UserService>(loggingHolder.Function);
        activity?.SetTag("page", page);

        var cacheKey = $"{CacheKey}:{loggingHolder.Function}:page={page}";
        
        var cached = await _cache.GetAsync<List<UserDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", loggingHolder.Class, loggingHolder.Function, cacheKey);
            return new UserTransferGridDTO(cached!); 
        }

        var result = await _repo.GetRecordsByPaginationAsync(page, cancellationToken);
        if (result == null || result.Count <= 0)  
        { 
            LogWarningNoRecord(loggingHolder);
            return new UserTransferGridDTO([]); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new UserTransferGridDTO(result); 
    }

    public async Task<UserTransferDTO> GetRecordByIdAsync(int id, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(UserService), nameof(GetRecordByIdAsync));

        using Activity? activity = TracingConstants.StartApiActivity<UserService>(loggingHolder.Function);
        activity?.SetTag("id", id);

        var cacheKey = $"{CacheKey}:{loggingHolder.Function}:id={id}";
        
        var cached = await _cache.GetAsync<UserDTO>(cacheKey);
        if (cached != null && cached.Id > 0) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(UserService), loggingHolder.Function, cacheKey);
            return new UserTransferDTO(cached); 
        }
        else if (cached != null) { await _cache.RemoveAsync(cacheKey); }

        var result = await _repo.GetRecordByIdAsync(id, cancellationToken);
        if (result == null || result.Id <= 0) 
        { 
            _logger.LogWarning("{class}.{function}: No record found for {id}", nameof(UserService), loggingHolder.Function, id);
            return new UserTransferDTO(new()); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new UserTransferDTO( result); 
    }

    public async Task<UserTransferDTO> GetUsernameAsync(string username, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(UserService), nameof(GetUsernameAsync));

        using Activity? activity = TracingConstants.StartApiActivity<UserService>(nameof(GetUsernameAsync));
        activity?.SetTag("username", username);

        var cacheKey = $"{CacheKey}:{loggingHolder.Function}:id={username}";
        
        var cached = await _cache.GetAsync<UserDTO>(cacheKey);
        if (cached != null && cached.Id > 0) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", loggingHolder.Class, loggingHolder.Function, cacheKey);
            return new UserTransferDTO(cached); 
        }
        else if (cached != null) { await _cache.RemoveAsync(cacheKey); }

        var result = await _repo.GetUsernameAsync(username, cancellationToken);
        if (result == null || result.Id <= 0) 
        { 
            _logger.LogWarning("{class}.{function}: No record found for {id}", loggingHolder.Class, loggingHolder.Function, username);
            return new UserTransferDTO(new()); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new UserTransferDTO(result); 
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        LoggingHolder loggingHolder = new(nameof(UserService), nameof(CreateRecordAsync));

        using Activity? activity = TracingConstants.StartApiActivity<UserService>(loggingHolder.Function);
        activity?.SetTag("dto.type", nameof(CreateUserDTO));

        var result = await _repo.CreateRecordAsync(dto, cancellationToken);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log}", loggingHolder.Class, loggingHolder.Function, "EntityNotCreated");
            return result ?? TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated);
        }
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto, CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, cancellationToken);

        LoggingHolder loggingHolder = new(nameof(UserService), nameof(UpdateRecordAsync));

        using Activity? activity = TracingConstants.StartApiActivity<UserService>(loggingHolder.Function);
        activity?.SetTag("dto.Id", dto.Id);
        activity?.SetTag("dto.type", nameof(UpdateUserDTO));

        var result = await _repo.UpdateRecordAsync(dto, cancellationToken);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log} for {Id}", loggingHolder.Class, loggingHolder.Function, "EntityNotUpdated", dto.Id);
            return result ?? TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated);
        }

        await _cache.RemoveAsync($"{CacheKey}:GetRecordByIdAsync:id={dto.Id}");
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken)
    {
        LoggingHolder loggingHolder = new(nameof(UserService), nameof(DeleteRecordByIdAsync));

        using Activity? activity = TracingConstants.StartApiActivity<UserService>(loggingHolder.Function);
        activity?.SetTag("id", id);

        var result = await _repo.DeleteRecordByIdAsync(id, cancellationToken);
        if (!result)
        {
            _logger.LogWarning("{class}.{function}: No record deleted for {Id}", nameof(UserService), nameof(DeleteRecordByIdAsync), id);
        }

        await _cache.RemoveAsync($"{CacheKey}:GetRecordByIdAsync:id={id}");
        return result;
    }
}
