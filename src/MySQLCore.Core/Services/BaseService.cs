namespace MySQLCore.Core.Services;

public abstract class BaseService
{
    protected readonly ILogger<BaseService> _logger = default!;
    protected readonly ICacheService _cache = default!;
    protected readonly TimeSpan timeSpan = TimeSpan.FromMinutes(5);
    protected readonly string CacheKey = "NO_CACHE_KEY";


    public BaseService(ILogger<BaseService> logger, ICacheService cache, string cacheKey = "NO_CACHE_KEY")
    {
        _logger = logger;
        _cache = cache;
        CacheKey = cacheKey;
    }

    public static string SerializePayload(object payload)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(payload);
    }

    public void LogWarningNoRecord(LoggingHolder loggingHolder)
    {
        _logger.LogWarning("{class}.{function}: No records found", loggingHolder.Class, loggingHolder.Function);
    }

}