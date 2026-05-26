namespace MySQLCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutboxMessagerController : BaseController
{
    private readonly IOutboxMessagerService _service = default!; 

    public OutboxMessagerController(ILogger<OutboxMessagerController> logger,IOutboxMessagerService service) : base(logger) 
    {
        _service = service;
    }

    [HttpGet("GetPendingMessages")]
    public async Task<IActionResult> GetPendingMessages(int take = 10)
    {
        var messages = await _service.GetPendingAsync(take);
        return Ok(messages);
    }

    [HttpGet("GetLatestPublishedMessage")]
    public async Task<IActionResult> GetLatestPublishedOutboxMessage()
    {
        var messages = await _service.GetLatestPublishedOutboxMessage();
        return Ok(messages);
    }

    [HttpGet("GetOutboxMessage/{id}")]
    public async Task<IActionResult> GetOutboxMessage(long id)
    {
        var messages = await _service.GetOutboxMessage(id);
        return Ok(messages);
    }

}