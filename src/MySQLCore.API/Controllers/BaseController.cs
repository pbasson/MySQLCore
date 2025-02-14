using Microsoft.AspNetCore.Mvc;

namespace MySQLCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController(ILogger<BaseController> logger) : ControllerBase
    {
        public readonly ILogger<BaseController> _logger = logger;
    }
}
