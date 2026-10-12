namespace MySQLCore.Core.Messager.Models;

public class ImageGalleryMessage(int imageId, string fileName) : IMessage
{
    public Guid MessageId { get; set; }
    public int ImageId { get; set; } = imageId;
    public string FileName { get; set; } = fileName;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
