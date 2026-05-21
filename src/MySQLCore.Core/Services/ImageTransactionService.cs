namespace MySQLCore.Core.Services;

public class ImageTransactionService : BaseService, IImageTransactionService
{
    private readonly IImageTransactionRepo _repo = default!;

    public ImageTransactionService(ILogger<ImageTransactionService> logger, IImageTransactionRepo repo): base(logger)
    {
        _repo = repo;
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
        return result;
    }

    public async Task<TransferDTO> UpdateRecordAsync(UpdateImageTransactionDTO dto)
    {
        var result = await _repo.UpdateRecordAsync(dto);
        return result;
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        var result = await _repo.DeleteRecordByIdAsync(id);
        return result;
    }

}
