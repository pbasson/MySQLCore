namespace MySQLCore.Infrastructure.Repos.TransactionRepo;

public class CRUDTransactionRepo : BaseRepo, ICRUDTransactionRepo 
{
    public CRUDTransactionRepo(MySQLCoreDBContext dBContext) : base(dBContext) { }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsAsync() 
    {
        var results = await _dBContext.CRUDTransaction.OrderByDescending(x => x.Id).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync();
        return results ?? [];
    }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsPaginationAsync(int page) 
    {
        var settings = new PageSettings();
        var results = await _dBContext.CRUDTransaction.OrderBy(x=>x.Id).Skip(settings.SkipCount(page))
            .Take(settings.PageSize).AsNoTracking().Select(x => x.ToMapped()).ToListAsync();
        return results ?? [];
    }

    public async Task<CRUDTransactionDTO> GetRecordByIdAsync(int id) 
    {
        var result = await _dBContext.CRUDTransaction.FirstOrDefaultAsync(x => x.Id == id);
        return result != null ? result.ToMapped() : new();
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateCRUDTransactionDTO dto) 
    {
        if (dto.IsNull()) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using var activity = TracingConstants.RepoActivitySource.StartActivity("CRUDTransactionRepo.CreateRecordAsync");
        activity?.SetTag("dto.type", nameof(CreateCRUDTransactionDTO));


        await _semaphore.WaitAsync();

        try
        {
            var mapped = dto.ToEntity();
            _dBContext.CRUDTransaction.Add(mapped);
            await SaveChangesAsync();

            return new TransferDTO( mapped.Id, string.Empty, ServiceResultType.Success);

        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateCRUDTransactionDTO dto) 
    {
        if ( dto.IsNull() ) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using var activity = TracingConstants.RepoActivitySource.StartActivity("CRUDTransactionRepo.UpdateRecordAsync");
        activity?.SetTag("dto.ImageTransactionID", dto.Id);
        activity?.SetTag("dto.type", nameof(UpdateCRUDTransactionDTO));

        await _semaphore.WaitAsync();

        try
        {
            // await Task.Delay(1000); // Simulating long running operation, to test semaphore locking.
            CRUDTransaction? existModel = await FindRecordByIdAsync(dto.Id);
            if(existModel == null ) 
            { 
                return TransferFactory.GetTransferFailure(TransferEnum.EntityNotExist);    
            }
                
            var mapped = dto.ToEntity();
            existModel.SetCreated(mapped);
            UpdateEntity(existModel, mapped);
            await SaveChangesAsync();
            return new TransferDTO( mapped.Id, string.Empty, ServiceResultType.Success );
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteRecordByIdAsync(int id) 
    {
        await _semaphore.WaitAsync();

        try
        {
            CRUDTransaction? existModel = await FindRecordByIdAsync(id);
            if(existModel.IsNull() ) { return false; }
            else if (existModel != null) {
                _dBContext.CRUDTransaction.Remove(existModel);
                return await SaveChangesAsync();
            }

            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task<CRUDTransaction?> FindRecordByIdAsync(int id) {
        var result = await _dBContext.CRUDTransaction.FindAsync(id);
        return result.IsNotNull() ? result : null;
    }
}
