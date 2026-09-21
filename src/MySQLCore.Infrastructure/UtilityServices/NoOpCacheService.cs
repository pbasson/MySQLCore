namespace MySQLCore.Infrastructure.UtilityServices;

public sealed class NoOpCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key) => Task.FromResult<T?>(default);

    public Task SetAsync<T>(string key, T value, TimeSpan ttl) => Task.CompletedTask;

    public Task RemoveAsync(string key) => Task.CompletedTask;
}
