namespace MySQLCore.Infrastructure.Factory;

public static class ImageFactoryExtension
{
    public static ImageTransaction ToEntity(this ImageTransactionDTO dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
        ImageGalleries = dto.ImageGalleries?.Select(x => ToEntity(x)).ToList() ?? []
    };

    public static ImageGallery ToEntity(ImageGalleryDTO dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        ImageTransactionID = dto.ImageTransactionID,
        ImagePath = dto.ImagePath,
    };

    public static ImageTransaction ToEntity(this CreateImageTransactionDTO dto) => new()
    {
        ImageType = dto.ImageType,
        ImageGalleries = dto.ImageGalleries?.Select(x => ToEntity(x)).ToList() ?? []
    };

    public static ImageGallery ToEntity(this CreateImageGalleryDTO dto) => new()
    {
        ImagePath = dto.ImagePath,
    };

    public static ImageGallery ToEntity(int id, string? imagePath) => new()
    {
        ImageGalleryId = 0,
        ImageTransactionID = id,
        ImagePath = imagePath,
    };

    public static ImageTransaction ToEntity(this UpdateImageTransactionDTO dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        ImageGalleries = dto.ImageGalleries?.Select(x => ToEntity(x)).ToList() ?? []
    };

    public static ImageTransactionDTO ToMapped(this ImageTransaction dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
        ImageGalleries = dto.ImageGalleries?.Select(x => ToMapped(x)).ToList() ?? []
    };

    public static ImageGalleryDTO ToMapped(ImageGallery dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        ImageTransactionID = dto.ImageTransactionID,
        ImagePath = dto.ImagePath,
    };
}
