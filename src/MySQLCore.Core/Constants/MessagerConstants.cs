namespace MySQLCore.Core.Constants;

public static class MessagerConstants
{
    public const string IMAGE_QUEUE = "image-processing-queue";
    public const string RABBITMQ_SERVICE = "rabbitmq";

    public static string MessagerPath( string input = "")
    {
        return !string.IsNullOrEmpty(input) ? input : "localhost" ;
    }

    public static string RabbitMQService()
    {
        return MessagerPath(RABBITMQ_SERVICE);
    }
}