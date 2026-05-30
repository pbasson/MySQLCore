namespace MySQLCore.Core.Models.DTOs.ImageDTOs;

public class ImageTransactionDTO : BaseDTO 
{
    public int ImageTransactionID { get; set; }
    public string? ImageType { get; set; }
    public List<ImageGalleryDTO>? ImageGalleries { get; set; } 
}

public class ImageGalleryDTO 
{
    public int ImageGalleryId { get; set; }
    public int ImageTransactionID { get; set; }
    public string? ImagePath { get; set; }
}

public class CreateImageTransactionDTO 
{
    public string? ImageType { get; set; }
    public List<CreateImageGalleryDTO>? ImageGalleries { get; set; } 
}

public class CreateImageGalleryDTO 
{
    public string? ImagePath { get; set; }
}

public class UpdateImageTransactionDTO  {
    public int ImageTransactionID { get; set; }
    public string? ImageType { get; set; }
    public List<ImageGalleryDTO>? ImageGalleries { get; set; } 
}

public class TransferImageTransactionDTO : BaseTransfer
{
    public ImageTransactionDTO? Record { get; set; }

    public TransferImageTransactionDTO(ActionStatusType statusType, ImageTransactionDTO? record = null)
    {
        ActionStatusType = statusType;
        Record = record;
    }
}

public class TransferImageTransactionGridDTO : BaseTransfer
{
    public List<ImageTransactionDTO>? Records { get; set; }

    public TransferImageTransactionGridDTO(ActionStatusType statusType, List<ImageTransactionDTO>? records = null)
    {
        ActionStatusType = statusType;
        Records = records;
    }
}
