namespace MySQLCore.Core.Models.DTOs.Image_Gallery;

public class TransferImageGalleryDTO : BaseTransfer
{
    public ImageGalleryDTO? Record { get; set; }

    public TransferImageGalleryDTO(ImageGalleryDTO? record = null)
    {
        Record = record;
    }
}

public class TransferImageGalleryGridDTO : BaseTransfer
{
    public List<ImageGalleryDTO>? Records { get; set; }

    public TransferImageGalleryGridDTO( List<ImageGalleryDTO>? records = null)
    {
        Records = records;
    }
}
