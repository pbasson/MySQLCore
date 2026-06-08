using MySQLCore.Infrastructure.Entities.Image_Gallery;

namespace MySQLCore.Infrastructure.Repos.TransactionRepo;

public class ImageGalleryRepo : BaseRepo, IImageGalleryRepo
{
    public ImageGalleryRepo(MySQLCoreDBContext dBContext): base(dBContext) { }

    public async Task<List<ImageGalleryDTO>> GetAllRecordsAsync() 
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(GetAllRecordsAsync));

        var results = await _dBContext.ImageGallery.OrderByDescending(x => x.ImageGalleryId).Take(100)  
            .Include(x => x.ImageFile).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync();
        return results ?? [];
    }

    public async Task<List<ImageGalleryDTO>> GetRecordsByPaginationAsync(int page) 
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(GetRecordsByPaginationAsync));
        activity?.SetTag("page", page);

        var settings = new PageSettings();
        var results = await _dBContext.ImageGallery.OrderBy(x => x.ImageGalleryId).Skip(settings.SkipCount(page))
            .Take(settings.PageSize).Include(x => x.ImageFile).AsNoTracking()
            .Select(x => x.ToMapped()).ToListAsync();
        return results ?? [];
    }

    public async Task<ImageGalleryDTO?> GetRecordByIdAsync(int id)  
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(GetRecordByIdAsync));
        activity?.SetTag("id", id);

        var result = await _dBContext.ImageGallery.Include(x => x.ImageFile).FirstOrDefaultAsync(x => x.ImageGalleryId == id);
        return result?.ToMapped();
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageGalleryDTO dto)
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
                await _dBContext.SaveChangesAsync();

                var message = new ImageCreatedMessage(mapped.ImageGalleryId, dto.GalleryName!);
                var outbox = OutboxMessageTransfer.GetTransfer( message.MessageId, nameof(CreateImageGalleryDTO), message);

                _dBContext.OutboxMessage.Add(outbox);
                await _dBContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new TransferDTO( mapped.ImageGalleryId, string.Empty, ServiceResultType.Success, message.MessageId );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageGalleryDTO dto)
    {
        if ( dto.IsNull() ) { return TransferFactory.GetTransferFailure(TransferEnum.DTONull); }

        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(UpdateRecordAsync));
        activity?.SetTag("dto.ImageTransactionID", dto.ImageGalleryId);
        activity?.SetTag("dto.type", nameof(UpdateImageGalleryDTO));

        ImageGallery? existDTO = await FindRecord(dto.ImageGalleryId);

        if ( existDTO == null ) { return TransferFactory.GetTransferFailure(TransferEnum.EntityNotExist); }

        var strategy = _dBContext.Database.CreateExecutionStrategy();   

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dBContext.Database.BeginTransactionAsync();

            try
            {
                existDTO.GalleryName = dto.GalleryName;

                var incomingFiles = dto.ImageFile ?? [];
                var incomingExistingIds = incomingFiles.Where(IsImageGalleryValid()).Select(x => x.ImageGalleryId).ToHashSet();

                var removeList = existDTO.ImageFile!.Where(x => !incomingExistingIds.Contains(x.ImageGalleryId)).ToList();
                if (removeList.Count > 0) { _dBContext.ImageFile.RemoveRange(removeList); }

                foreach (var incomingGallery in incomingFiles.Where(IsImageGalleryValid()))
                {
                    var existingGallery = existDTO.ImageFile!.FirstOrDefault(x => x.ImageGalleryId == incomingGallery.ImageGalleryId);
                    if (existingGallery == null) { continue; }

                    existingGallery.ImagePath = incomingGallery.ImagePath;
                }

                var addList = incomingFiles.Where(x => x.ImageGalleryId == 0)
                    .Select(x => ToEntity(existDTO.ImageGalleryId, x.ImageName, x.ImagePath)).ToList();

                if (addList.Count > 0) { _dBContext.ImageFile.AddRange(addList); }
                await SaveChangesAsync();

                var exportMessage = new ImageCreatedMessage( existDTO.ImageGalleryId, existDTO.GalleryName!);

                _dBContext.OutboxMessage.Add(OutboxMessageTransfer.GetTransfer(exportMessage.MessageId, nameof(UpdateImageGalleryDTO), exportMessage));
                await SaveChangesAsync();

                await transaction.CommitAsync();

                return new TransferDTO(existDTO.ImageGalleryId
                
                , string.Empty, ServiceResultType.Success, exportMessage.MessageId);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
    
    private static Func<ImageFileDTO, bool> IsImageGalleryValid()
    {
        return x => x.ImageFileId > 0;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)  
    {
        using Activity? activity = TracingConstants.StartApiActivity<ImageGalleryRepo>(nameof(DeleteRecordByIdAsync));
        activity?.SetTag("id", id);

        await _semaphore.WaitAsync();

        try
        {
            ImageGallery? existDTO = await FindRecord(id);
            if(existDTO.IsNull() ) { return false; }
            else if (existDTO != null) 
            {
                _dBContext.ImageGallery.Remove(existDTO);
                return await SaveChangesAsync();
            }

            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task<ImageGallery?> FindRecord(int id) 
    {
        var result = await _dBContext.ImageGallery.Include(x => x.ImageFile)
            .FirstOrDefaultAsync(x => x.ImageGalleryId == id);
        return result.IsNotNull() ? result : null;
    }
    
    private ImageFile ToEntity(int id, string? imageName, string? imagePath) => new()
    {
        ImageFileId = 0,
        ImageGalleryId = id,
        ImageName = imageName,
        ImagePath = imagePath,
    };
}
