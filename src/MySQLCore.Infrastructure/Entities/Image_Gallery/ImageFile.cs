namespace MySQLCore.Infrastructure.Entities.Image_Gallery;

public class ImageFile 
{
    [Key]
    public int ImageFileId { get; set; }

    [ForeignKey(nameof(ImageGallery))]
    public int ImageGalleryId { get; set; }
    public string? ImageName { get; set; }
    public int ImagePosition { get; set; }

    public virtual ImageGallery? ImageGallery {get; set;}
}
