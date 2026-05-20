namespace MySQLCore.Core.Services;

public class ImageService
{
    private readonly IMessagePublisher _publisher;

    public ImageService(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task CreateImageAsync(int imageId, string fileName)
    {
        var message = new ImageCreatedMessage( imageId, fileName, new Guid() );

        await _publisher.PublishAsync(MessagerConstants.IMAGE_QUEUE, message);
    }
}