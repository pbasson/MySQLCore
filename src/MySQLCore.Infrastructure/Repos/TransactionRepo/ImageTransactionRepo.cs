namespace MySQLCore.Infrastructure.Repos.TransactionRepo;

public class ImageTransactionRepo : BaseRepo, IImageTransactionRepo
{
    public ImageTransactionRepo(MySQLCoreDBContext dBContext): base(dBContext) { }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsAsync() 
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionRepo>(nameof(GetAllRecordsAsync));

        var results = await _dBContext.ImageTransaction.OrderByDescending(x => x.ImageTransactionID).Take(100)  
            .Include(x => x.ImageGalleries).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync();
        return results ?? [];
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionRepo>(nameof(GetAllRecordsPaginationAsync));
        activity?.SetTag("page", page);

        var settings = new PageSettings();
        var results = await _dBContext.ImageTransaction.OrderBy(x => x.ImageTransactionID).Skip(settings.SkipCount(page))
            .Take(settings.PageSize).Include(x => x.ImageGalleries).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync();
        return results ?? [];
    }

    public async Task<ImageTransactionDTO?> GetRecordByIdAsync(int id)  
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionRepo>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _dBContext.ImageTransaction.Include(x => x.ImageGalleries)
            .FirstOrDefaultAsync(x => x.ImageTransactionID == id);
        return result?.ToMapped();
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageTransactionDTO dto)
    {
        if (dto.IsNull()) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionRepo>(nameof(CreateRecordAsync));
        activity?.SetTag("dto.type", nameof(CreateImageTransactionDTO));

        var strategy = _dBContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dBContext.Database.BeginTransactionAsync();

            try
            {
                var mapped = dto.ToEntity();

                _dBContext.ImageTransaction.Add(mapped);
                await _dBContext.SaveChangesAsync();

                var message = new ImageCreatedMessage(mapped.ImageTransactionID, dto.ImageType!);
                var outbox = OutboxMessageTransfer.GetTransfer( message.MessageId, nameof(CreateImageTransactionDTO), message);

                _dBContext.OutboxMessage.Add(outbox);
                await _dBContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new TransferDTO( mapped.ImageTransactionID, string.Empty, ServiceResultType.Success, message.MessageId );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto)
    {
        if ( dto.IsNull() ) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionRepo>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto.ImageTransactionID", dto.ImageTransactionID);
        activity?.SetTag("dto.type", nameof(UpdateImageTransactionDTO));

        ImageTransaction? existDTO = await FindRecord(dto.ImageTransactionID);

        if ( existDTO == null ) { return TransferFactory.GetTransferFailure(TransferEnum.EntityNotExist); }

        var strategy = _dBContext.Database.CreateExecutionStrategy();   

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dBContext.Database.BeginTransactionAsync();

            try
            {
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
                    .Select(x => ToEntity(existDTO.ImageTransactionID, x.ImagePath)).ToList();

                if (addList.Count > 0) { _dBContext.ImageGallery.AddRange(addList); }
                await SaveChangesAsync();

                var exportMessage = new ImageCreatedMessage( existDTO.ImageTransactionID, existDTO.ImageType!);

                _dBContext.OutboxMessage.Add(OutboxMessageTransfer.GetTransfer(exportMessage.MessageId, nameof(UpdateImageTransactionDTO), exportMessage));
                await SaveChangesAsync();

                await transaction.CommitAsync();

                return new TransferDTO(existDTO.ImageTransactionID, string.Empty, ServiceResultType.Success, exportMessage.MessageId);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
    
    private static Func<ImageGalleryDTO, bool> IsImageGalleryValid()
    {
        return x => x.ImageGalleryId > 0;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)  
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageTransactionRepo>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        await _semaphore.WaitAsync();

        try
        {
            ImageTransaction? existDTO = await FindRecord(id);
            if(existDTO.IsNull() ) { return false; }
            else if (existDTO != null) 
            {
                _dBContext.ImageTransaction.Remove(existDTO);
                return await SaveChangesAsync();
            }

            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task<ImageTransaction?> FindRecord(int id) 
    {
        var result = await _dBContext.ImageTransaction.Include(x => x.ImageGalleries)
            .FirstOrDefaultAsync(x => x.ImageTransactionID == id);
        return result.IsNotNull() ? result : null;
    }
    private ImageGallery ToEntity(int id, string? imagePath) => new()
    {
        ImageGalleryId = 0,
        ImageTransactionID = id,
        ImagePath = imagePath,
    };
}
