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
    public async Task<ActionResult<List<ImageTransactionDTO>>> GetAllRecords() 
    {
        var result = await _service.GetAllRecordsAsync();
        return Ok(result);
    }

    [HttpGet("by-page/{page:int}")]
    public async Task<ActionResult<List<ImageTransactionDTO>>> GetAllRecordsPaginationAsync(int page) 
    {
        if (page.IsNotZero())  
        {
            var result = await _service.GetAllRecordsPaginationAsync(page);
            return Ok(result);
        }
        return BadRequest(); 
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ImageTransactionDTO>> GetRecordById(int id) 
    {
        if (id.IsNotZero())  
        {
            var result = await _service.GetRecordByIdAsync(id);
            if (result.IsNotNull()) { return (result.ImageTransactionID > 0) ? result : NotFound(); } 
        }
        return BadRequest(); 
    }

    [HttpPost("create")]
    public async Task<ActionResult<TransferDTO>> CreateRecord(CreateImageTransactionDTO dTO) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.CreateRecordAsync(dTO);
        return result.IsNotNull() ? Ok(result) : BadRequest();
    }

    [HttpPut("update")]
    public async Task<ActionResult<TransferDTO>> UpdateRecord(UpdateImageTransactionDTO dTO) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.UpdateRecordAsync(dTO);
        return result.IsNotNull() ? Ok(result) : BadRequest();
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
