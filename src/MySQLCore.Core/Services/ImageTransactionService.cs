namespace MySQLCore.Core.Services;

public class ImageTransactionService : IImageTransactionService
{
    private readonly ILogger<ImageTransactionService> _logger = default!;
    private readonly IImageTransactionRepo _repo = default!;
    private readonly IMessagePublisher _publisher;

    public ImageTransactionService(ILogger<ImageTransactionService> logger, IImageTransactionRepo repo, IMessagePublisher publisher)
    {
        _logger = logger;
        _repo = repo;
        _publisher = publisher;
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsAsync()
    {
        var result = await _repo.GetAllRecordsAsync();
        return result;
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page)
    {
        var result = await _repo.GetAllRecordsPaginationAsync(page);
        return result;
    }

    public async Task<ImageTransactionDTO> GetRecordByIdAsync(int id)
    {
        var result = await _repo.GetRecordByIdAsync(id);
        return result;
    }

    public async Task<TransferDTO> CreateRecordAsync(CreateImageTransactionDTO dto)
    {
        var result = await _repo.CreateRecordAsync(dto);

        if(!result.Success) { return result; }

        await _publisher.PublishAsync(MessagerConstants.IMAGE_QUEUE, new ImageCreatedMessage( result.Id, dto.ImageType! ));

        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto)
    {
        var result = await _repo.UpdateRecordAsync(dto);
        if(!result.Success) { return result; }

        await _publisher.PublishAsync(MessagerConstants.IMAGE_QUEUE, new ImageCreatedMessage( result.Id, dto.ImageType! ));

        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        var result = await _repo.DeleteRecordByIdAsync(id);
        return result;
    }

}
