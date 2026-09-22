using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MySqlConnector;
using MySQLCore.Core.Enums;
using MySQLCore.Core.Interfaces.Messager.Repo;
using MySQLCore.Core.Messager.Models;
using MySQLCore.Infrastructure.Context;
using MySQLCore.Infrastructure.Repos.MessagerRepo;
using Xunit;

namespace MySQLCore.Messaging.IntegrationTest;

public sealed class MySqlFactAttribute : FactAttribute
{
    public MySqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MYSQLCORE_TEST_CONNECTION")))
            Skip = "Set MYSQLCORE_TEST_CONNECTION to a disposable MySQL server with database creation permissions.";
    }
}

public sealed class ProcessedMessageTests : IAsyncLifetime
{
    private string? _connection;

    public async Task InitializeAsync()
    {
        var configured = Environment.GetEnvironmentVariable("MYSQLCORE_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(configured)) return;
        var builder = new MySqlConnectionStringBuilder(configured)
        {
            Database = "issue6_" + Guid.NewGuid().ToString("N")
        };
        _connection = builder.ConnectionString;
        await using var db = CreateContext();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (_connection == null) return;
        await using var db = CreateContext();
        await db.Database.EnsureDeletedAsync();
    }

    private MySQLCoreDBContext CreateContext() => new(new DbContextOptionsBuilder<MySQLCoreDBContext>()
        .UseMySql(_connection!, new MySqlServerVersion(new Version(8, 3, 0)),
            options => options.EnableRetryOnFailure()).Options);

    private async Task<MessageProcessResult> Process(ImageCreatedMessage message)
    {
        await using var db = CreateContext();
        var repo = new ProcessedMessageRepo(db, NullLogger<IProcessedMessageRepo>.Instance);
        return await repo.ProcessImageCreatedAsync(message, CancellationToken.None);
    }

    [MySqlFact]
    public async Task RedeliveryAfterCommitPreservesCompletion()
    {
        var message = new ImageCreatedMessage(1, "gallery") { MessageId = Guid.NewGuid() };
        Assert.Equal(MessageProcessResult.Completed, await Process(message));
        await using var db = CreateContext();
        var first = await db.ProcessedMessage.AsNoTracking().SingleAsync();
        Assert.Equal(ProcessMessageStatus.Processed, first.Status);
        Assert.Equal(MessageProcessResult.Duplicate, await Process(message));
        var second = await db.ProcessedMessage.AsNoTracking().SingleAsync();
        Assert.Equal(first.ProcessedAt, second.ProcessedAt);
        Assert.Equal(first.Status, second.Status);
    }

    [MySqlFact]
    public async Task ConcurrentDeliveriesCompleteOnlyOnce()
    {
        var message = new ImageCreatedMessage(2, "gallery") { MessageId = Guid.NewGuid() };
        var results = await Task.WhenAll(Enumerable.Range(0, 12).Select(_ => Process(message)));
        Assert.Single(results, x => x == MessageProcessResult.Completed);
        Assert.Equal(11, results.Count(x => x == MessageProcessResult.Duplicate));
        await using var db = CreateContext();
        Assert.Equal(1, await db.ProcessedMessage.CountAsync());
    }

    [MySqlFact]
    public async Task UnfinishedRecordResumesInsteadOfBeingSkipped()
    {
        var message = new ImageCreatedMessage(3, "gallery") { MessageId = Guid.NewGuid() };
        await using var db = CreateContext();
        db.ProcessedMessage.Add(new ProcessedMessageTransfer().GetTransfer(
            message.MessageId, nameof(ImageCreatedMessage), "ImageTransaction", message.ImageId));
        await db.SaveChangesAsync();
        Assert.Equal(MessageProcessResult.Completed, await Process(message));
        Assert.Equal(ProcessMessageStatus.Processed,
            (await db.ProcessedMessage.AsNoTracking().SingleAsync()).Status);
    }

    [MySqlFact]
    public async Task FailureBeforeCommitRollsBackClaimAndAllowsRetry()
    {
        var message = new ImageCreatedMessage(4, "gallery") { MessageId = Guid.NewGuid() };
        await using var db = CreateContext();
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TRIGGER reject_completion BEFORE UPDATE ON ProcessedMessage
            FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Injected completion failure'
            """);
        await Assert.ThrowsAsync<MySqlException>(() => Process(message));
        Assert.Equal(0, await db.ProcessedMessage.CountAsync());
        await db.Database.ExecuteSqlRawAsync("DROP TRIGGER reject_completion");
        Assert.Equal(MessageProcessResult.Completed, await Process(message));
    }

    [MySqlFact]
    public async Task ReusedIdForDifferentEntityIsRejected()
    {
        var message = new ImageCreatedMessage(5, "gallery") { MessageId = Guid.NewGuid() };
        await Process(message);
        message.ImageId = 6;
        await Assert.ThrowsAsync<InvalidOperationException>(() => Process(message));
        await using var db = CreateContext();
        Assert.Equal(5, (await db.ProcessedMessage.SingleAsync()).EntityId);
    }
}
