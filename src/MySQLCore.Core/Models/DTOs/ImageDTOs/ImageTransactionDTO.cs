namespace MySQLCore.Core.Models.DTOs.ImageDTOs;

public class ImageTransactionDTO : BaseDTO {
    public int ImageTransactionID { get; set; }
    public string? ImageType { get; set; }
    public List<ImageGalleryDTO>? ImageGalleries { get; set; } 
}

public class ImageGalleryDTO {
    public int ImageGalleryId { get; set; }
    public int ImageTransactionID { get; set; }
    public string? ImagePath { get; set; }
}

public class CreateImageTransactionDTO {
    public string? ImageType { get; set; }
    public List<CreateImageGalleryDTO>? ImageGalleries { get; set; } 
}

public class CreateImageGalleryDTO {
    public string? ImagePath { get; set; }
}

public class UpdateImageTransactionDTO  {
    public int ImageTransactionID { get; set; }
    public string? ImageType { get; set; }
    public List<ImageGalleryDTO>? ImageGalleries { get; set; } 
}

