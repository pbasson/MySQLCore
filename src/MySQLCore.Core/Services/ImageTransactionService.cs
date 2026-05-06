using Microsoft.Extensions.Logging;

namespace MySQLCore.Core.Services;

public class ImageTransactionService : IImageTransactionService
{
    private readonly IImageTransactionRepo _repo = default!;
    private readonly IMessagePublisher _publisher;
    private readonly IProcessedMessageRepo _processedMessageRepo;
    private readonly ILogger<ImageTransactionService> _logger = default!;

    public ImageTransactionService(ILogger<ImageTransactionService> logger, IImageTransactionRepo repo, IMessagePublisher publisher, IProcessedMessageRepo processedMessageRepo)
    {
        _logger = logger;
        _repo = repo;
        _publisher = publisher;
        _processedMessageRepo = processedMessageRepo;
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


        await ImageProcessAsync(result.Id, dto.ImageType!);
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto)
    {
        var result = await _repo.UpdateRecordAsync(dto);
        if(!result.Success) { return result; }

        await ImageProcessAsync(result.Id, dto.ImageType!);

        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        var result = await _repo.DeleteRecordByIdAsync(id);
        return result;
    }

    private async Task ImageProcessAsync(int imageId, string fileName)
    {
        var message = new ImageCreatedMessage( imageId, fileName );
        await ProcessAsync(message);
    }

    public async Task ProcessAsync(ImageCreatedMessage message)
{
    if (await _processedMessageRepo.ExistsAsync(message.MessageId))
    {
        _logger.LogInformation("Duplicate message ignored: {MessageId}", message.MessageId);
        return;
    }

    await _publisher.PublishAsync(MessagerConstants.IMAGE_QUEUE, message);

    await _processedMessageRepo.AddAsync(message.MessageId);
}

}
