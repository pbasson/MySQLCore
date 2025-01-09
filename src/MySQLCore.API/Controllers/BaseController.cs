using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MySQLCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly ILogger<BaseController> _logger = default!;

        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
    }
}
