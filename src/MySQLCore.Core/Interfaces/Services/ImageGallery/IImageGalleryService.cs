namespace MySQLCore.Core.Interfaces.Services.ImageGallery;

public interface IImageGalleryService
{
    Task<TransferImageGalleryGridDTO> GetAllRecordsAsync();
    Task<TransferImageGalleryGridDTO> GetRecordsByPaginationAsync(int page);
    Task<TransferImageGalleryDTO> GetRecordByIdAsync(int id);
    Task<TransferImageGalleryGridDTO> GetRecordsByGalleryNameAsync(string galleryName);
    Task<TransferDTO> CreateRecordAsync(CreateImageGalleryDTO dto);
    Task<TransferDTO> UpdateRecordAsync(UpdateImageGalleryDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
