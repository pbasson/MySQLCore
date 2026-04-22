using MySQLCore.Infrastructure.Factory;

namespace MySQLCore.Infrastructure.Repos;

public class CRUDTransactionRepo : BaseRepo, ICRUDTransactionRepo 
{
    private readonly CRUDFactory _factory = new();
    public CRUDTransactionRepo(MySQLCoreDBContext dBContext) : base(dBContext) { }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsAsync() 
    {
        var results = await _dBContext.CRUDTransaction.OrderByDescending(x => x.Id).AsNoTracking()
            .Select(x => _factory.ToMapped(x)).ToListAsync();
        return results ?? [];
    }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsPaginationAsync(int page) 
    {
        var settings = new PageSettings();
        var results = await _dBContext.CRUDTransaction.OrderBy(x=>x.Id).Skip(settings.SkipCount(page))
            .Take(settings.PageSize).AsNoTracking().Select(x => _factory.ToMapped(x)).ToListAsync();
        return results ?? [];
    }

    public async Task<CRUDTransactionDTO> GetRecordByIdAsync(int id) 
    {
        var result = await _dBContext.CRUDTransaction.FirstOrDefaultAsync(x => x.Id == id);
        return result != null ? new CRUDFactory().ToMapped(result) : new();
    }

    public async Task<bool> CreateRecordAsync(CreateCRUDTransactionDTO dto) 
    {
        if(dto.IsNull() ) { return false; }
        
        var mapped = new CRUDFactory().ToEntity(dto);
        _dBContext.CRUDTransaction.Add(mapped);
        return await SaveChangesAsync();
    }

    public async Task<bool> UpdateRecordAsync(UpdateCRUDTransactionDTO dto) 
    {
        if(dto.IsNull() ) { return false; }

        CRUDTransaction? existDTO = await FindRecordByIdAsync(dto.Id);
        if(existDTO.IsNull() ) { return false; }
        else if (existDTO != null)
        {
            var mapped = new CRUDFactory().ToEntity(dto);
            existDTO.SetCreated(mapped);
            UpdateEntity(existDTO, mapped);
            return await SaveChangesAsync();
        }

        return false;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id) 
    {
        CRUDTransaction? existDTO = await FindRecordByIdAsync(id);
        if(existDTO.IsNull() ) { return false; }
        else if (existDTO != null) {
            _dBContext.CRUDTransaction.Remove(existDTO);
            return await SaveChangesAsync();
        }

        return false;
    }
    
    private async Task<CRUDTransaction?> FindRecordByIdAsync(int id) {
        var result = await _dBContext.CRUDTransaction.FindAsync(id);
        return result.IsNotNull() ? result : null;
    }
}
