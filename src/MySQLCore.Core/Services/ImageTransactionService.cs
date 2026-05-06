namespace MySQLCore.Core.Services;

public class ImageTransactionService : IImageTransactionService
{
    private readonly IImageTransactionRepo _repo = default!;
    private readonly IMessagePublisher _publisher;

    public ImageTransactionService(IImageTransactionRepo repo, IMessagePublisher publisher)
    {
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

        await CreateImageAsync(result.Id, dto.ImageType!);
        return result;
    }

    public async Task<bool> UpdateRecordAsync(UpdateImageTransactionDTO dto)
    {
        var result = await _repo.UpdateRecordAsync(dto);
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        var result = await _repo.DeleteRecordByIdAsync(id);
        return result;
    }

    private async Task CreateImageAsync(int imageId, string fileName)
    {
        var message = new ImageCreatedMessage( imageId, fileName );
        await _publisher.PublishAsync(MessagerConstants.IMAGE_QUEUE, message);
    }

}
