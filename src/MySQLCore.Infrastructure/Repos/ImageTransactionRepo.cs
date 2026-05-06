namespace MySQLCore.Infrastructure.Repos;

public class ImageTransactionRepo : BaseRepo, IImageTransactionRepo
{
    ImageFactory _factory = new();

    public ImageTransactionRepo(MySQLCoreDBContext dBContext): base(dBContext) { }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsAsync() 
    {
        var results = await _dBContext.ImageTransaction.OrderByDescending(x => x.ImageTransactionID)
            .Include(x => x.ImageGalleries).AsNoTracking()
            .Select(x => _factory.ToMapped(x)).ToListAsync();
        return results ?? [];
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page) 
    {
        var settings = new PageSettings();
        var results = await _dBContext.ImageTransaction.OrderBy(x => x.ImageTransactionID).Skip(settings.SkipCount(page))
            .Take(settings.PageSize).Include(x => x.ImageGalleries).AsNoTracking()
            .Select(x => _factory.ToMapped(x)).ToListAsync();
        return results ?? [];
    }

    public async Task<ImageTransactionDTO> GetRecordByIdAsync(int id)  
    {
        var result = await _dBContext.ImageTransaction.Include(x => x.ImageGalleries)
            .FirstOrDefaultAsync(x => x.ImageTransactionID == id);
        return result != null ? _factory.ToMapped(result) : new();
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageTransactionDTO dto) 
    {
        if ( dto.IsNull() ) { return new TransferDTO(0, "DTO is null"); }

        var mapped = _factory.ToEntity(dto);
        _dBContext.ImageTransaction.Add(mapped);
        var result = await SaveChangesAsync();
        
        if(!result) { return new TransferDTO(0, "Save Changes Not Executed"); }

        return new TransferDTO(mapped.ImageTransactionID); 
    }

    public async Task<bool> UpdateRecordAsync(UpdateImageTransactionDTO dto) 
    {
        if (dto.IsNull()) { return false; }
    
        ImageTransaction? existDTO = await FindRecord(dto.ImageTransactionID);
        
        if (existDTO != null) 
        {
            var mapped = _factory.ToEntity(dto);
            existDTO.SetCreated(mapped);
            UpdateEntity(existDTO, mapped);
            
            if( existDTO.IsGallery() && mapped.IsGallery() )
            {
                List<ImageGallery> RemoveList = existDTO.ImageGalleries!.Where(x => !mapped.ImageGalleries!
                    .Any(z => z.ImageGalleryId == x.ImageGalleryId)).ToList();
                if (RemoveList.Count > 0) { _dBContext.RemoveRange(RemoveList); }

                List<ImageGallery> AddList = mapped.ImageGalleries!.Where(x => x.ImageGalleryId == 0)
                    .Select(x => _factory.ToEntity(mapped.ImageTransactionID, x.ImagePath)).ToList();
                if (AddList.Count > 0) { _dBContext.ImageGallery.AddRange(AddList); }
            }

            return await SaveChangesAsync();
        }
        return false;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)  
    {
        ImageTransaction? existDTO = await FindRecord(id);
        if ( existDTO != null ) {
            _dBContext.ImageTransaction.Remove(existDTO);
            return await SaveChangesAsync();
        }

        return false;
    }
    
    private async Task<ImageTransaction?> FindRecord(int id) 
    {
        var result = await _dBContext.ImageTransaction.Include(x => x.ImageGalleries)
            .FirstOrDefaultAsync(x => x.ImageTransactionID == id);
        return result.IsNotNull() ? result : null;
    }
}
