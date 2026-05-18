namespace MySQLCore.Core.Services;

public abstract class BaseService
{
    protected readonly ILogger<BaseService> _logger = default!;

    public BaseService(ILogger<BaseService> logger)
    {
        _logger = logger;
    }
}