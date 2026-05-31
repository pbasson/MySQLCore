namespace MySQLCore.Infrastructure.Repos.TransactionRepo;

public class UserRepo : BaseRepo, IUserRepo 
{
    public UserRepo(MySQLCoreDBContext dBContext) : base(dBContext) { }

    public async Task<List<UserDTO>> GetAllRecordsAsync() 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(GetAllRecordsAsync));

        var results = await _dBContext.User.OrderByDescending(x => x.Id).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync();
        return results ?? [];
    }

    public async Task<List<UserDTO>> GetAllRecordsPaginationAsync(int page) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(GetAllRecordsPaginationAsync));
        activity?.SetTag("page", page);
        
        var settings = new PageSettings();
        var results = await _dBContext.User.OrderBy(x=>x.Id).Skip(settings.SkipCount(page))
            .Take(settings.PageSize).AsNoTracking().Select(x => x.ToMapped()).ToListAsync();
        return results ?? [];
    }

    public async Task<UserDTO?> GetRecordByIdAsync(int id) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _dBContext.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return result?.ToMapped();
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateUserDTO dto) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(CreateRecordAsync));
        activity?.SetTag("dto.type", nameof(CreateUserDTO));
        
        if (dto.IsNull()) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        await _semaphore.WaitAsync();

        try
        {
            var mapped = dto.ToEntity();
            _dBContext.User.Add(mapped);
            await SaveChangesAsync();

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

    public async Task<TransferDTO> UpdateRecordAsync(UpdateUserDTO dto) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto.ImageTransactionID", dto.Id);
        activity?.SetTag("dto.type", nameof(UpdateUserDTO));

        if ( dto.IsNull() ) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        await _semaphore.WaitAsync();

        try
        {
            // await Task.Delay(1000); // Simulating long running operation, to test semaphore locking.
            User? existModel = await FindRecordByIdAsync(dto.Id);
            if(existModel == null ) 
            { 
                return TransferFactory.GetTransferFailure(TransferEnum.EntityNotExist);    
            }

            var emailExists = await _dBContext.User.AnyAsync(x => x.Email == dto.Email);

            if (emailExists)
            {
                return TransferFactory.GetTransferFailure(TransferEnum.Conflict);
            }
                
            var mapped = dto.ToEntity();
            existModel.SetCreated(mapped);
            UpdateEntity(existModel, mapped);
            await SaveChangesAsync();
            return new TransferDTO( mapped.Id, string.Empty, ServiceResultType.Success );
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

    public async Task<bool> DeleteRecordByIdAsync(int id) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<UserRepo>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        await _semaphore.WaitAsync();

        try
        {
            User? existModel = await FindRecordByIdAsync(id);
            if(existModel.IsNull() ) { return false; }
            else if (existModel != null) {
                _dBContext.User.Remove(existModel);
                return await SaveChangesAsync();
            }

            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task<User?> FindRecordByIdAsync(int id) {
        var result = await _dBContext.User.FindAsync(id);
        return result.IsNotNull() ? result : null;
    }
}
