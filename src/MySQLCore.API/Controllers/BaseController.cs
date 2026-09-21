namespace MySQLCore.API.Controllers;

public abstract class BaseController(ILogger<BaseController> logger) : ControllerBase
{
    public readonly ILogger<BaseController> _logger = logger;

    protected ObjectResult TransferFailure(TransferDTO result) => result.ServiceResultType switch
    {
        MySQLCore.Core.Enums.ServiceResultType.Conflict => Conflict(result),
        MySQLCore.Core.Enums.ServiceResultType.NotFound => NotFound(result),
        _ => StatusCode(StatusCodes.Status500InternalServerError, result)
    };

}
