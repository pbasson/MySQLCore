namespace MySQLCore.API.Controllers;

[Route("api/image-transactions")]
[ApiController]
public class ImageTransactionController : BaseController
{
    private readonly IImageTransactionService _service;
    public ImageTransactionController(IImageTransactionService service, ILogger<ImageTransactionController> logger) : base(logger) {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    public async Task<ActionResult<TransferImageTransactionGridDTO>> GetAllRecords() 
    {
        var result = await _service.GetAllRecordsAsync();
        return TransferActionResult(result);
    }

    [HttpGet("by-page/{page:int}")]
    public async Task<ActionResult<TransferImageTransactionGridDTO>> GetAllRecordsPaginationAsync(int page) 
    {
        if (page.IsNotZero())  
        {
            var result = await _service.GetAllRecordsPaginationAsync(page);
            return TransferActionResult(result);
        }
        return BadRequest(); 
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransferImageTransactionDTO>> GetRecordById(int id) 
    {
        if (id.IsNotZero())  
        {
            var result = await _service.GetRecordByIdAsync(id);
            return TransferActionResult(result);

            // if (result.IsNotNull()) { return (result.ImageTransactionID > 0) ? result : NotFound(); } 
        }
        return BadRequest(); 
    }

    [HttpPost("create")]
    public async Task<ActionResult<TransferDTO>> CreateRecord(CreateImageTransactionDTO dTO) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.CreateRecordAsync(dTO);
        return TransferActionResult(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<TransferDTO>> UpdateRecord(UpdateImageTransactionDTO dTO) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.UpdateRecordAsync(dTO);
        return TransferActionResult(result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<bool>> DeleteRecord(int id) 
    {
        if (id.IsNotZero()) 
        {
            var result = await _service.DeleteRecordByIdAsync(id);
            return result.IsNotNull() ? Ok(result) : BadRequest();
        }
        return BadRequest();
    }
}
