using MySQLCore.Infrastructure.Entities.Image_Gallery;

namespace MySQLCore.Infrastructure.Factory;

public static class ImageGalleryExtension
{
    public static ImageGallery ToEntity(this ImageGalleryDTO dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        GalleryName = dto.GalleryName,
        GalleryPath = dto.GalleryPath,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
        ImageFile = dto.ImageFile?.Select(x => ToEntity(x)).ToList() ?? []
    };

    public static ImageFile ToEntity(ImageFileDTO dto) => new()
    {
        ImageFileId = dto.ImageFileId,
        ImageGalleryId = dto.ImageGalleryId,
        ImageName = dto.ImageName,
        ImagePosition = dto.ImagePosition,
    };

    public static ImageGallery ToEntity(this CreateImageGalleryDTO dto) => new()
    {
        GalleryName = dto.GalleryName,
        GalleryPath = dto.GalleryPath,
        ImageFile = dto.ImageFile?.Select(x => ToEntity(x)).ToList() ?? []
    };

    public static ImageFile ToEntity(this CreateImageFileDTO dto) => new()
    {
        ImageName = dto.ImageName,
        ImagePosition = dto.ImagePosition,
    };

    public static ImageFile ToEntity(int id, string? imageName, int imagePosition) => new()
    {
        ImageFileId = 0,
        ImageGalleryId = id,
        ImageName = imageName,
        ImagePosition = imagePosition,
    };

    public static ImageGallery ToEntity(this UpdateImageGalleryDTO dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        GalleryName = dto.GalleryName,
        GalleryPath = dto.GalleryPath,
        ImageFile = dto.ImageFile?.Select(x => ToEntity(x)).ToList() ?? []
    };

    public static ImageGalleryDTO ToMapped(this ImageGallery dto) => new()
    {
        ImageGalleryId = dto.ImageGalleryId,
        GalleryName = dto.GalleryName,
        GalleryPath = dto.GalleryPath,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
        ImageFile = dto.ImageFile?.OrderBy(x => x.ImagePosition).Select(x => ToMapped(x)).ToList() ?? []
    };

    public static ImageFileDTO ToMapped(ImageFile dto) => new()
    {
        ImageFileId = dto.ImageFileId,
        ImageGalleryId = dto.ImageGalleryId,
        ImageName = dto.ImageName,
        ImagePosition = dto.ImagePosition,
    };
}
