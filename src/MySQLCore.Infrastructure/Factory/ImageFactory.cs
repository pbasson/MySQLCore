namespace MySQLCore.Infrastructure.Factory;

public class ImageFactory
{
    public ImageTransaction ToEntity(ImageTransactionDTO dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
        ImageGalleries = dto.ImageGalleries?.Select(x => ToEntity(x)).ToList() ?? []
    };

    public ImageGallery ToEntity(ImageGalleryDTO dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        ImageTransactionID = dto.ImageTransactionID,
        ImagePath = dto.ImagePath,
    };

    public ImageTransaction ToEntity(CreateImageTransactionDTO dto) => new()
    {
        ImageType = dto.ImageType,
        ImageGalleries = dto.ImageGalleries?.Select(x => ToEntity(x)).ToList() ?? []
    };

    public ImageGallery ToEntity(CreateImageGalleryDTO dto) => new()
    {
        ImagePath = dto.ImagePath,
    };

    public ImageGallery ToEntity(int id, string? imagePath) => new()
    {
        ImageGalleryId = 0,
        ImageTransactionID = id,
        ImagePath = imagePath,
    };


    public ImageTransaction ToEntity(UpdateImageTransactionDTO dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        ImageGalleries = dto.ImageGalleries?.Select(x => ToEntity(x)).ToList() ?? []
    };


    public ImageTransactionDTO ToMapped(ImageTransaction dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
        ImageGalleries = dto.ImageGalleries?.Select(x => ToMapped(x)).ToList() ?? []
    };

    public ImageGalleryDTO ToMapped(ImageGallery dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        ImageTransactionID = dto.ImageTransactionID,
        ImagePath = dto.ImagePath,
    };
}

