namespace MySQLCore.Infrastructure.Entities.Tables.ImageTables;

public class ImageTransaction : BaseModel
{
    [Key]
    public int ImageTransactionID { get; set; }
    public string? ImageType { get; set; }
    public virtual List<ImageGallery>? ImageGalleries { get; set; } 

    public bool IsGallery() {
        return ( ImageGalleries != null && ImageGalleries.Any() ) ? true : false;
    }
}

public class ImageGallery 
{
    [Key]
    public int ImageGalleryId { get; set; }

    [ForeignKey(nameof(ImageTransaction))]
    public int ImageTransactionID { get; set; }

    public string? ImagePath { get; set; }

    public virtual ImageTransaction? ImageTransaction {get; set;}

}
