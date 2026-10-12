using System.Text.Json;
using MySQLCore.Core.Messager.Models;
using Xunit;

namespace MySQLCore.Messaging.IntegrationTest;

public sealed class MessageSerializationTests
{
    [Fact]
    public void RedeliveryPreservesOriginalMessageId()
    {
        var original = new ImageGalleryMessage(1, "gallery") { MessageId = Guid.NewGuid() };
        var payload = JsonSerializer.Serialize(original);

        var first = JsonSerializer.Deserialize<ImageGalleryMessage>(payload);
        var redelivery = JsonSerializer.Deserialize<ImageGalleryMessage>(payload);

        Assert.Equal(original.MessageId, first!.MessageId);
        Assert.Equal(original.MessageId, redelivery!.MessageId);
    }

    [Theory]
    [InlineData("{\"ImageId\":1,\"FileName\":\"gallery\"}")]
    [InlineData("{\"MessageId\":\"00000000-0000-0000-0000-000000000000\",\"ImageId\":1,\"FileName\":\"gallery\"}")]
    public void MissingOrEmptyIdIsNotReplaced(string payload)
    {
        var message = JsonSerializer.Deserialize<ImageGalleryMessage>(payload);

        Assert.NotNull(message);
        Assert.Equal(Guid.Empty, message.MessageId);
    }
}
