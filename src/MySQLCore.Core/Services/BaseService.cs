namespace MySQLCore.Core.Services;

public abstract class BaseService
{
    protected readonly ILogger<BaseService> _logger = default!;

    protected readonly ICacheService _cache = default!;

    public BaseService(ILogger<BaseService> logger, ICacheService cache)
    {
        _logger = logger;
        _cache = cache;
    }
}