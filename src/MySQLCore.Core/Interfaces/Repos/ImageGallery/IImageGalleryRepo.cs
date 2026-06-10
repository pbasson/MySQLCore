namespace MySQLCore.Core.Interfaces.Repos.ImageGallery;

public interface IImageGalleryRepo
{
    Task<List<ImageGalleryDTO>> GetAllRecordsAsync();
    Task<List<ImageGalleryDTO>> GetRecordsByPaginationAsync(int page);
    Task<ImageGalleryDTO?> GetRecordByIdAsync(int id);
    Task<List<ImageGalleryDTO>> GetRecordsByGalleryNameAsync(string galleryName);
    Task<TransferDTO> CreateRecordAsync(CreateImageGalleryDTO dto);
    Task<TransferDTO> UpdateRecordAsync(UpdateImageGalleryDTO dto);
    Task<bool> DeleteRecordByIdAsync(int id);
}
