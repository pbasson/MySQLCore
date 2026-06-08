namespace MySQLCore.Infrastructure.Entities.Image_Gallery;

public class ImageGallery : BaseModel
{
    [Key]
    public int ImageGalleryId { get; set; }
    public string? GalleryName { get; set; }
    public string? GalleryPath { get; set; }
    public virtual List<ImageFile>? ImageFile { get; set; } 

    public bool IsFiles() 
    {
        return ImageFile != null && ImageFile.Any() ;
    }
}
