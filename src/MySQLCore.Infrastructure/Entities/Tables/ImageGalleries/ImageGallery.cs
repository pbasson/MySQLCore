namespace MySQLCore.Infrastructure.Entities.Tables.ImageTables;

public class ImageGallery : BaseModel
{
    [Key]
    public int ImageGalleryId { get; set; }
    public string? ImageGalleryName { get; set; }
    public virtual List<ImageFile>? ImageFiles { get; set; } 

    public bool IsFiles() {
        return ( ImageFiles != null && ImageFiles.Any() ) ? true : false;
    }
}

public class ImageFile 
{
    [Key]
    public int ImageFileId { get; set; }

    [ForeignKey(nameof(ImageGallery))]
    public int ImageGalleryId { get; set; }
    public string? ImageFile { get; set; }
    public string? ImagePath { get; set; }

    public virtual ImageGallery? ImageTransaction {get; set;}
}
