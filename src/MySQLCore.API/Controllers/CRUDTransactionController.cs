namespace MySQLCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CRUDTransactionController : BaseController
{
    private readonly ICRUDTransactionService _service;
    public CRUDTransactionController(ICRUDTransactionService service, ILogger<CRUDTransactionController> logger) : base(logger) {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    [Route("GetAllRecords")]
    public async Task<ActionResult<List<CRUDTransactionDTO>>> GetAllRecords() {
        var result = await _service.GetAllRecordsAsync();
        return Ok(result);
    }

    [HttpGet]
    [Route("GetAllRecordsPaginationAsync/{page:int}")]
    public async Task<ActionResult<List<CRUDTransactionDTO>>> GetAllRecordsPaginationAsync(int page) 
    {
        if ( page.IsNotZero() )  
        {
            var result = await _service.GetAllRecordsPaginationAsync(page);
            return Ok(result);
        }
        return BadRequest(); 
    }

    [HttpGet]
    [Route("GetRecordById/{Id:int}")]
    public async Task<ActionResult<CRUDTransactionDTO>> GetRecordById(int Id) 
    {
        if ( Id.IsNotZero() )  
        {
            var result = await _service.GetRecordByIdAsync(Id);
            if (result.IsNotNull()) { return (result.Id > 0) ? result : NotFound(); } 
        }
        return BadRequest(); 
    }

    [HttpPost]
    [Route("CreateRecord")]
    public async Task<ActionResult<TransferDTO>> CreateRecord(CreateCRUDTransactionDTO dTO) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.CreateRecordAsync(dTO);
        return result.IsNotNull() ? Ok(result) : BadRequest();
    }

    [HttpPut]
    [Route("UpdateRecord")]
    public async Task<ActionResult<TransferDTO>> UpdateRecord(UpdateCRUDTransactionDTO dTO) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.UpdateRecordAsync(dTO);
        return result.IsNotNull() ? Ok(result) : BadRequest();
    }

    [HttpDelete]
    [Route("DeleteRecord")]
    public async Task<ActionResult<bool>> DeleteRecord(int id) 
    {
        if (id.IsNotZero()) {
            var result = await _service.DeleteRecordByIdAsync(id);
            return result.IsNotNull() ? Ok(result) : BadRequest();
        }
        return BadRequest();
    }
}
