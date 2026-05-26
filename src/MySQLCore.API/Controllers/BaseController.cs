namespace MySQLCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController(ILogger<BaseController> logger) : ControllerBase
    {
        public readonly ILogger<BaseController> _logger = logger;

        protected ActionResult<TransferDTO> TransferActionResult(TransferDTO? result)
        {
            if (result == null) { return BadRequest(); }

            return result.ActionStatusType switch
            {
                ActionStatusType.Ok => Ok(result),
                ActionStatusType.Created => StatusCode(StatusCodes.Status201Created, result),
                ActionStatusType.NoContent => NoContent(),
                ActionStatusType.NotFound => NotFound(result),
                ActionStatusType.BadRequest => BadRequest(result),
                ActionStatusType.Conflict => Conflict(result),
                ActionStatusType.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, result),
                ActionStatusType.Failed => BadRequest(result),
                _ => result.Success ? Ok(result) : BadRequest(result)
            };
        }
    }
}
