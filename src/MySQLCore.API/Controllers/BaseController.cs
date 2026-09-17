namespace MySQLCore.API.Controllers;

public abstract class BaseController(ILogger<BaseController> logger) : ControllerBase
{
    public readonly ILogger<BaseController> _logger = logger;

}
