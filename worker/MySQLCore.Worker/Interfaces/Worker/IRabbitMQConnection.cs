namespace MySQLCore.Worker.Interfaces.Worker;

public interface IRabbitMQConnection 
{
    Task<IChannel> CreateChannelAsync(CancellationToken stoppingToken, bool publisherConfirmations = false);
}
