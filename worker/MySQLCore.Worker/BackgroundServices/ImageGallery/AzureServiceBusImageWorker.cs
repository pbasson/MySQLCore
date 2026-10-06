namespace MySQLCore.Worker.BackgroundServices.ImageGallery;

public class AzureServiceBusImageWorker : BaseWorker<ImageGalleryMessage>
{
    public AzureServiceBusImageWorker(ILogger<BaseWorker<ImageGalleryMessage>> logger, IOptions<MessagerSettings> messagerSettings,
        IRabbitMQConnection rabbitMQConnection) : base(logger, messagerSettings, rabbitMQConnection)
    {
        
    }
}