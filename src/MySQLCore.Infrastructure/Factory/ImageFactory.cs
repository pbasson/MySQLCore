namespace MySQLCore.Infrastructure.Factory;

public class ImageFactory
{
    public ImageTransaction Create(ImageTransactionDTO dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
        ImageGalleries = dto.ImageGalleries?.Select(x => new ImageFactory().Create(x)).ToList() ?? []
    };

    public ImageGallery Create(ImageGalleryDTO dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        ImageTransactionID = dto.ImageTransactionID,
        ImagePath = dto.ImagePath,
    };

    public ImageTransaction Create(CreateImageTransactionDTO dto) => new()
    {
        ImageType = dto.ImageType,
        ImageGalleries = dto.ImageGalleries?.Select(x => new ImageFactory().Create(x)).ToList() ?? []
    };

    public ImageGallery Create(CreateImageGalleryDTO dto) => new()
    {
        ImagePath = dto.ImagePath,
    };

    public ImageTransaction Create(UpdateImageTransactionDTO dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        ImageGalleries = dto.ImageGalleries?.Select(x => new ImageFactory().Create(x)).ToList() ?? []
    };


    public ImageTransactionDTO Mapped(ImageTransaction dto) => new()
    {
        ImageTransactionID = dto.ImageTransactionID,
        ImageType = dto.ImageType,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
        ImageGalleries = dto.ImageGalleries?.Select(x => new ImageFactory().Mapped(x)).ToList() ?? []
    };

    public ImageGalleryDTO Mapped(ImageGallery dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        ImageTransactionID = dto.ImageTransactionID,
        ImagePath = dto.ImagePath,
    };
}

