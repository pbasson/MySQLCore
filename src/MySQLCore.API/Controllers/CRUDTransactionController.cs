namespace MySQLCore.API.Controllers;

[Route("api/crud-transactions")]
[ApiController]
public class CRUDTransactionController : BaseController
{
    private readonly ICRUDTransactionService _service;
    public CRUDTransactionController(ICRUDTransactionService service, ILogger<CRUDTransactionController> logger) : base(logger) 
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    public async Task<ActionResult<List<CRUDTransactionDTO>>> GetAllRecords() 
    {
        var result = await _service.GetAllRecordsAsync();
        return Ok(result);
    }

    [HttpGet("by-page/{page:int}")]
    public async Task<ActionResult<List<CRUDTransactionDTO>>> GetAllRecordsPaginationAsync(int page) 
    {
        if ( page.IsNotZero() )  
        {
            var result = await _service.GetAllRecordsPaginationAsync(page);
            return Ok(result);
        }
        return BadRequest(); 
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CRUDTransactionDTO>> GetRecordById(int Id) 
    {
        if ( Id.IsNotZero() )  
        {
            var result = await _service.GetRecordByIdAsync(Id);
            if (result.IsNotNull()) { return (result.Id > 0) ? result : NotFound(); } 
        }
        return BadRequest(); 
    }

    [HttpPost("create")]
    public async Task<ActionResult<TransferDTO>> CreateRecord(CreateCRUDTransactionDTO dTO) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.CreateRecordAsync(dTO);
        return TransferActionResult(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<TransferDTO>> UpdateRecord(UpdateCRUDTransactionDTO dTO) 
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
