namespace MySQLCore.Core.Messager.Models;

public class ImageCreatedMessage : IMessage
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public int ImageId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ImageCreatedMessage(int imageId, string fileName )
    {
        ImageId = imageId;
        FileName = fileName;
    }
}
