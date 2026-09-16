namespace MySQLCore.Core.Interfaces.Repos.ImageGallery;

public interface IImageGalleryRepo
{
    Task<List<ImageGalleryDTO>> GetAllRecordsAsync(CancellationToken cancellationToken);
    Task<List<ImageGalleryDTO>> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken);
    Task<ImageGalleryDTO?> GetRecordByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<ImageGalleryDTO>> GetRecordsByGalleryNameAsync(string galleryName, CancellationToken cancellationToken);
    Task<TransferDTO> CreateRecordAsync(CreateImageGalleryDTO dto, CancellationToken cancellationToken);
    Task<TransferDTO> UpdateRecordAsync(UpdateImageGalleryDTO dto, CancellationToken cancellationToken);
    Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken);
}
