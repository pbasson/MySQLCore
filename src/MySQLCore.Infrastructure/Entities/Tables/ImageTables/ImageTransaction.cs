using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySQLCore.Infrastructure.Entities.Tables;

public class ImageTransaction : BaseModel
{
    [Key]
    public int ImageTransactionID { get; set; }
    public string? ImageType { get; set; }
    public virtual List<ImageGallery>? ImageGalleries { get; set; } 
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
