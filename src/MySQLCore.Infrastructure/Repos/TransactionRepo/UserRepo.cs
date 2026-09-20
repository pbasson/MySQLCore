namespace MySQLCore.Infrastructure.Repos.TransactionRepo;

public sealed class UserRepo : BaseRepo<IUserRepo>, IUserRepo 
{
    public UserRepo(MySQLCoreDBContext dBContext, ILogger<UserRepo> logger) : base(dBContext, logger)
    {
        _logger = logger;
    }

    public async Task<List<UserDTO>> GetLatestRecordsAsync(CancellationToken cancellationToken) 
    {
        int take = 50;

        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(GetLatestRecordsAsync));

        var results = await _dBContext.User.OrderByDescending(x => x.Id).Take(take).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync(cancellationToken);
        return results ?? [];
    }


    public async Task<List<UserDTO>> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(GetRecordsByPaginationAsync));
        activity?.SetTag("page", page);
        
        var settings = new PageSettings();
        var results = await _dBContext.User.OrderBy(x=>x.Id).Skip(settings.SkipCount(page))
            .Take(settings.PageSize).AsNoTracking().Select(x => x.ToMapped()).ToListAsync(cancellationToken);
        return results ?? [];
    }


    public async Task<UserDTO?> GetRecordByIdAsync(int id, CancellationToken cancellationToken) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _dBContext.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return result?.ToMapped();
    }

    public async Task<UserDTO?> GetUsernameAsync(string username, CancellationToken cancellationToken = default) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(GetUsernameAsync));
        activity?.SetTag("username", username);

        var result = await _dBContext.User.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);
        return result?.ToMapped();
    }

    public async Task<bool> CheckEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(CheckEmailAsync));
        activity?.SetTag("email", email);

        return await _dBContext.User.AsNoTracking().AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto, CancellationToken cancellationToken) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(CreateRecordAsync));
        activity?.SetTag("dto.type", nameof(CreateUserDTO));
        
        if (dto.IsNull()) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        await _semaphore.WaitAsync();

        try
        {
            var mapped = dto.ToEntity();
            _dBContext.User.Add(mapped);
            await SaveChangesAsync(cancellationToken);

            return new TransferDTO( mapped.Id, string.Empty, ServiceResultType.Success);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
        {
            return TransferFactory.GetTransferFailure(TransferEnum.Conflict);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto, CancellationToken cancellationToken) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto.ImageTransactionID", dto.Id);
        activity?.SetTag("dto.type", nameof(UpdateUserDTO));

        if ( dto.IsNull() )
        { 
            _logger.LogWarning("{name}: Update failed: DTO is null", nameof(UserRepo));
            return TransferFactory.GetTransferFailure(TransferEnum.DTONull);
        }

        await _semaphore.WaitAsync();

        try
        {
            User? existModel = await FindRecordByIdAsync(dto.Id, cancellationToken);
            if (existModel == null)
            {
                return TransferFactory.GetTransferFailure(TransferEnum.EntityNotExist);
            }

            var mapped = dto.ToEntity();
            existModel.SetCreated(mapped);
            UpdateEntity(existModel, mapped);
            await SaveChangesAsync(cancellationToken);
            return new TransferDTO(mapped.Id, string.Empty, ServiceResultType.Success);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
        {
            _logger.LogWarning("Update failed due to duplicate key: {Message}", ex.Message);
            return TransferFactory.GetTransferFailure(TransferEnum.Conflict);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        await _semaphore.WaitAsync();

        try
        {
            User? existModel = await FindRecordByIdAsync(id, cancellationToken);
            if(existModel.IsNull() ) { return false; }
            else if (existModel != null) {
                _dBContext.User.Remove(existModel);
                return await SaveChangesAsync(cancellationToken);
            }

            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task<User?> FindRecordByIdAsync(int id, CancellationToken cancellationToken) 
    {
        var result = await _dBContext.User.FindAsync([id], cancellationToken);
        return result.IsNotNull() ? result : null;
    }

}
