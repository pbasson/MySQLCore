namespace MySQLCore.Infrastructure.Repos.TransactionRepo;

public class ImageGalleryRepo : BaseRepo, IImageGalleryRepo
{
    public ImageGalleryRepo(MySQLCoreDBContext dBContext): base(dBContext) { }

    public async Task<List<ImageGalleryDTO>> GetAllRecordsAsync(CancellationToken cancellationToken) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(GetAllRecordsAsync));

        var results = await _dBContext.ImageGallery.OrderByDescending(x => x.ImageGalleryId).Take(100)  
            .Include(x => x.ImageFile).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync(cancellationToken);
        return results ?? [];
    }

    public async Task<List<ImageGalleryDTO>> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(GetRecordsByPaginationAsync));
        activity?.SetTag("page", page);

        var settings = new PageSettings();
        var results = await _dBContext.ImageGallery.OrderBy(x => x.ImageGalleryId).Skip(settings.SkipCount(page))
            .Take(settings.PageSize).Include(x => x.ImageFile).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync(cancellationToken);
        return results ?? [];
    }

    public async Task<ImageGalleryDTO?> GetRecordByIdAsync(int id, CancellationToken cancellationToken)  
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _dBContext.ImageGallery.Include(x => x.ImageFile!.OrderBy(x => x.ImagePosition))
        .FirstOrDefaultAsync(x => x.ImageGalleryId == id, cancellationToken);
        return result?.ToMapped();
    }

    public async Task<List<ImageGalleryDTO>> GetRecordsByGalleryNameAsync(string galleryName, CancellationToken cancellationToken) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(GetRecordsByPaginationAsync));
        activity?.SetTag("page", galleryName);

        var settings = new PageSettings();
        var results = await _dBContext.ImageGallery.Where(x => !string.IsNullOrEmpty(x.GalleryName) && x.GalleryName.Contains( galleryName)).OrderBy(x => x.GalleryName)
            .Select(x => x.ToMapped()).ToListAsync(cancellationToken);
        return results ?? [];
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageGalleryDTO dto, CancellationToken cancellationToken)
    {
        if (dto.IsNull()) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(CreateRecordAsync));
        activity?.SetTag("dto.type", nameof(CreateImageGalleryDTO));

        var strategy = _dBContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dBContext.Database.BeginTransactionAsync();

            try
            {
                var mapped = dto.ToEntity();

                _dBContext.ImageGallery.Add(mapped);
                await _dBContext.SaveChangesAsync(cancellationToken);

                var message = new ImageCreatedMessage(mapped.ImageGalleryId, dto.GalleryName!);
                var outbox = OutboxMessageTransfer.GetTransfer( message.MessageId, nameof(CreateImageGalleryDTO), message);

                _dBContext.OutboxMessage.Add(outbox);
                await _dBContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return new TransferDTO( mapped.ImageGalleryId, string.Empty, ServiceResultType.Success, message.MessageId );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageGalleryDTO dto, CancellationToken cancellationToken)
    {
        if ( dto.IsNull() ) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto.ImageGalleryId", dto.ImageGalleryId);
        activity?.SetTag("dto.type", nameof(UpdateImageGalleryDTO));

        ImageGallery? existDTO = await FindRecord(dto.ImageGalleryId, cancellationToken);

        if ( existDTO == null ) { return TransferFactory.GetTransferFailure(TransferEnum.EntityNotExist); }

        var strategy = _dBContext.Database.CreateExecutionStrategy();   

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dBContext.Database.BeginTransactionAsync();

            try
            {
                existDTO.GalleryName = dto.GalleryName;
                existDTO.GalleryPath = dto.GalleryPath;

                var incomingFiles = dto.ImageFile ?? [];
                var incomingExistingIds = incomingFiles.Where(IsImageFileValid()).Select(x => x.ImageFileId).ToHashSet();

                var removeList = existDTO.ImageFile!.Where(x => !incomingExistingIds.Contains(x.ImageFileId)).ToList();
                if (removeList.Count > 0) { _dBContext.ImageFile.RemoveRange(removeList); }

                foreach (var incomingFile in incomingFiles.Where(IsImageFileValid()))
                {
                    var existingFile = existDTO.ImageFile!.FirstOrDefault(x => x.ImageFileId == incomingFile.ImageFileId);
                    if (existingFile == null) { continue; }

                    existingFile.ImageName = incomingFile.ImageName;
                    existingFile.ImagePosition = incomingFile.ImagePosition;
                }

                var addList = incomingFiles.Where(x => x.ImageFileId == 0)
                    .Select(x => ToEntity(existDTO.ImageGalleryId, x.ImageName, x.ImagePosition)).ToList();

                if (addList.Count > 0) { _dBContext.ImageFile.AddRange(addList); }
                await SaveChangesAsync(cancellationToken);

                var exportMessage = new ImageCreatedMessage( existDTO.ImageGalleryId, existDTO.GalleryName!);

                _dBContext.OutboxMessage.Add(OutboxMessageTransfer.GetTransfer(exportMessage.MessageId, nameof(UpdateImageGalleryDTO), exportMessage));
                await SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync();

                return new TransferDTO(existDTO.ImageGalleryId, string.Empty, ServiceResultType.Success, exportMessage.MessageId);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
    
    private static Func<ImageFileDTO, bool> IsImageFileValid()
    {
        return x => x.ImageFileId > 0;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken)  
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            ImageGallery? existDTO = await FindRecord(id, cancellationToken);
            if(existDTO.IsNull() ) { return false; }
            else if (existDTO != null) 
            {
                _dBContext.ImageGallery.Remove(existDTO);
                return await SaveChangesAsync(cancellationToken);
            }

            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task<ImageGallery?> FindRecord(int id, CancellationToken cancellationToken) 
    {
        var result = await _dBContext.ImageGallery.Include(x => x.ImageFile)
            .FirstOrDefaultAsync(x => x.ImageGalleryId == id, cancellationToken);
        return result.IsNotNull() ? result : null;
    }
    
    private ImageFile ToEntity(int id, string? imageName, int imagePosition) => new()
    {
        ImageFileId = 0,
        ImageGalleryId = id,
        ImageName = imageName,
        ImagePosition = imagePosition,
    };
}
