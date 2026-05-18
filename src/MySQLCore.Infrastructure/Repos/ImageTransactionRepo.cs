namespace MySQLCore.Infrastructure.Repos;

public class ImageTransactionRepo : BaseRepo, IImageTransactionRepo
{
    ImageFactory _factory = new();
    private readonly IOutboxMessagerRepo _messagerRepo = default!;


    public ImageTransactionRepo(MySQLCoreDBContext dBContext, IOutboxMessagerRepo messagerRepo): base(dBContext)
    {
        _messagerRepo = messagerRepo;
    }

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
        if ( dto.IsNull() ) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using var activity = TracingConstants.RepoActivitySource.StartActivity("ImageTransactionRepo.CreateRecordAsync");
        activity?.SetTag("dto.ImageType", dto.ImageType);
        activity?.SetTag("dto.type", nameof(CreateImageTransactionDTO));

        var mapped = _factory.ToEntity(dto);
        _dBContext.ImageTransaction.Add(mapped);
        
        var result = await SaveChangesAsync();
        if(!result) { return TransferFactory.GetTransferFailure(TransferEnum.SaveChangesNotExecuted); }

        return new TransferDTO(mapped.ImageTransactionID, string.Empty, ServiceResultType.Success); 
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto)
    {
        if ( dto.IsNull() ) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using var activity = TracingConstants.RepoActivitySource.StartActivity("ImageTransactionRepo.UpdateRecordAsync");
        activity?.SetTag("dto.ImageTransactionID", dto.ImageTransactionID);
        activity?.SetTag("dto.type", nameof(UpdateImageTransactionDTO));

        var messageId = Guid.NewGuid();
        ImageTransaction? existDTO = await FindRecord(dto.ImageTransactionID);

        if ( existDTO == null ) { return TransferFactory.GetTransferFailure(TransferEnum.EntityNotExist); }

        existDTO.ImageType = dto.ImageType;

        var incomingGalleries = dto.ImageGalleries ?? [];
        var incomingExistingIds = incomingGalleries.Where(IsImageGalleryValid()).Select(x => x.ImageGalleryId).ToHashSet();

        var removeList = existDTO.ImageGalleries!.Where(x => !incomingExistingIds.Contains(x.ImageGalleryId)).ToList();
        if (removeList.Count > 0) { _dBContext.ImageGallery.RemoveRange(removeList); }

        foreach (var incomingGallery in incomingGalleries.Where(IsImageGalleryValid()))
        {
            var existingGallery = existDTO.ImageGalleries!.FirstOrDefault(x => x.ImageGalleryId == incomingGallery.ImageGalleryId);
            if (existingGallery == null) { continue; }

            existingGallery.ImagePath = incomingGallery.ImagePath;
        }

        var addList = incomingGalleries.Where(x => x.ImageGalleryId == 0)
            .Select(x => _factory.ToEntity(existDTO.ImageTransactionID, x.ImagePath)).ToList();

        if (addList.Count > 0) { _dBContext.ImageGallery.AddRange(addList); }

        _dBContext.OutboxMessage.Add(OutboxMessageTransfer.GetTransfer(messageId, nameof(UpdateImageTransactionDTO)));

        var result = await SaveChangesAsync();
        if(!result) { return TransferFactory.GetTransferFailure(TransferEnum.SaveChangesNotExecuted); }

        return new TransferDTO(existDTO.ImageTransactionID, string.Empty, ServiceResultType.Success, messageId);
    }

    private static Func<ImageGalleryDTO, bool> IsImageGalleryValid()
    {
        return x => x.ImageGalleryId > 0;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)  
    {
        ImageTransaction? existDTO = await FindRecord(id);
        if ( existDTO != null ) 
        {
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
