namespace MySQLCore.Core.Interfaces.Services.ImageGallery;

public interface IImageGalleryService
{
    Task<TransferImageGalleryGridDTO> GetAllRecordsAsync(CancellationToken cancellationToken);
    Task<TransferImageGalleryGridDTO> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken);
    Task<TransferImageGalleryDTO> GetRecordByIdAsync(int id, CancellationToken cancellationToken);
    Task<TransferImageGalleryGridDTO> GetRecordsByGalleryNameAsync(string galleryName, CancellationToken cancellationToken);
    Task<TransferDTO> CreateRecordAsync(CreateImageGalleryDTO dto, CancellationToken cancellationToken);
    Task<TransferDTO> UpdateRecordAsync(UpdateImageGalleryDTO dto, CancellationToken cancellationToken);
    Task<bool> DeleteRecordByIdAsync(int id, CancellationToken cancellationToken);
}
