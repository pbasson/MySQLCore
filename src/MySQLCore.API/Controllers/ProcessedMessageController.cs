namespace MySQLCore.API.Controllers;

[ApiController]
[Route("api/processed-message")]
public class ProcessedMessageController : BaseController
{
    private readonly IProcessedMessageService _service = default!;

    public ProcessedMessageController(ILogger<ProcessedMessageController> logger,IProcessedMessageService service) : base(logger)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMessage(Guid id )
    {
        var messages = await _service.GetMessage(id);
        return Ok(messages);
    }
 
    [HttpGet()]
    public async Task<IActionResult> GetLatestProcessedMessages()
    {
        var messages = await _service.GetLatestProcessedMessages();
        return Ok(messages);
    }
}
