
namespace MySQLCore.Infrastructure.Messager;

public class RabbitMQPublisher : IMessagePublisher
{
    public async Task PublishAsync<TMessage>(string queueName, TMessage message)
    {
        /// This Sets up the connect to the RabbitMQ for Queuing processing
        var factory = new ConnectionFactory { HostName = MessagerConstants.RabbitMQService(), UserName = "guest", Password = "password001" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync( queue: queueName, durable: true, exclusive: false, autoDelete: false);

        /// RabbitMQ only accepts Bytes hence payload must be serialized JSON to get bytes
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync( exchange: string.Empty, routingKey: queueName, body: body);
    }
}