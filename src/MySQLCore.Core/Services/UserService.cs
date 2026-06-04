namespace MySQLCore.Core.Services;

public class UserService : BaseService, IUserService 
{
    private readonly IUserRepo _repo = default!;

    public UserService(ILogger<UserService> logger, ICacheService cache,IUserRepo repo): base(logger, cache)
    {
        _repo = repo;
    }

    /// <summary>
    /// Get All Records 
    /// </summary>
    public async Task<UserTransferGridDTO> GetAllRecordsAsync()
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserService>(nameof(GetAllRecordsAsync));

        var cacheKey = $"user:GetAllRecordsAsync";

        var cached = await _cache.GetAsync<List<UserDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(UserService), nameof(GetAllRecordsAsync), cacheKey);
            return new UserTransferGridDTO(ActionStatusType.Ok, cached!); 
        }

        var result = await _repo.GetAllRecordsAsync();
        if (result == null || result.Count <= 0)  
        { 
            _logger.LogWarning("{class}.{function}: No records found", nameof(UserService), nameof(GetAllRecordsAsync));
            return new UserTransferGridDTO(ActionStatusType.NotFound, []); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new UserTransferGridDTO(ActionStatusType.Ok, result); 

    }

    public async Task<UserTransferGridDTO> GetAllRecordsPaginationAsync(int page)
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserService>(nameof(GetAllRecordsPaginationAsync));
        activity?.SetTag("page", page);

        var cacheKey = $"user:GetAllRecordsPaginationAsync:page={page}";
        
        var cached = await _cache.GetAsync<List<UserDTO>>(cacheKey);
        if (cached != null) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(UserService), nameof(GetAllRecordsPaginationAsync), cacheKey);
            return new UserTransferGridDTO(ActionStatusType.Ok, cached!); 
        }

        var result = await _repo.GetAllRecordsPaginationAsync(page);
        if (result == null || result.Count <= 0)  
        { 
            _logger.LogWarning("{class}.{function}: No records found", nameof(UserService), nameof(GetAllRecordsPaginationAsync));
            return new UserTransferGridDTO(ActionStatusType.NotFound, []); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new UserTransferGridDTO(ActionStatusType.Ok, result); 

    }

    public async Task<UserTransferDTO> GetRecordByIdAsync(int id)
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserService>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var cacheKey = $"user:GetRecordByIdAsync:id={id}";
        
        var cached = await _cache.GetAsync<UserDTO>(cacheKey);
        if (cached != null && cached.Id > 0) 
        { 
            _logger.LogInformation("{class}.{function}: Cache hit for {cacheKey}", nameof(UserService), nameof(GetRecordByIdAsync), cacheKey);
            return new UserTransferDTO(ActionStatusType.Ok, cached); 
        }
        else if (cached != null) { await _cache.RemoveAsync(cacheKey); }

        var result = await _repo.GetRecordByIdAsync(id);
        if (result == null || result.Id <= 0) 
        { 
            _logger.LogWarning("{class}.{function}: No record found for {id}", nameof(UserService), nameof(GetRecordByIdAsync), id);
            return new UserTransferDTO(ActionStatusType.NotFound, new()); 
        }

        await _cache.SetAsync(cacheKey, result, timeSpan);
        return new UserTransferDTO(ActionStatusType.Ok, result); 
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto)
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserService>(nameof(CreateRecordAsync));
        activity?.SetTag("dto", SerializePayload(dto));
        activity?.SetTag("dto.type", nameof(CreateUserDTO));

        var result = await _repo.CreateRecordAsync(dto);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log}", nameof(UserService), nameof(CreateRecordAsync), "EntityNotCreated");
            return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); 
        }
        await _cache.RemoveAsync("user:GetAllRecordsAsync");
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto)
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserService>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto", SerializePayload(dto));
        activity?.SetTag("dto.Id", dto.Id);
        activity?.SetTag("dto.type", nameof(UpdateUserDTO));

        var result = await _repo.UpdateRecordAsync(dto);
        if (result == null || !result.Success) 
        { 
            _logger.LogWarning("{class}.{function}: {log} for {Id}", nameof(UserService), nameof(UpdateRecordAsync), "EntityNotUpdated", dto.Id);
            return TransferFactory.GetTransferFailure(TransferEnum.EntityNotCreated); 
        }

        await _cache.RemoveAsync("user:GetAllRecordsAsync");
        await _cache.RemoveAsync($"user:GetRecordByIdAsync:id={dto.Id}");
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserService>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _repo.DeleteRecordByIdAsync(id);
        if (!result)
        {
            _logger.LogWarning("{class}.{function}: No record deleted for {Id}", nameof(UserService), nameof(DeleteRecordByIdAsync), id);
        }

        await _cache.RemoveAsync("user:GetAllRecordsAsync");
        await _cache.RemoveAsync($"user:GetRecordByIdAsync:id={id}");
        return result;
    }
    private static string SerializePayload(object payload)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(payload);
    }
}
