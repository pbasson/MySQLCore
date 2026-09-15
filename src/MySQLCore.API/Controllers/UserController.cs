namespace MySQLCore.API.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : BaseController
{
    private readonly IUserService _service;
    public UserController(IUserService service, ILogger<UserController> logger) : base(logger) 
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    // [HttpGet]
    // public async Task<ActionResult<UserTransferGridDTO>> GetAllRecords() 
    // {
    //     var result = await _service.GetAllRecordsAsync();
    //     return TransferActionResult(result);
    // }


    /// <summary>
    /// Get records by pagination 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    [HttpGet("by-page/{page:int}")]
    public async Task<ActionResult<UserTransferGridDTO>> GetRecordsByPagination(int page, CancellationToken cancellationToken) 
    {
        if ( page.IsNotZero() )  
        {
            var result = await _service.GetRecordsByPaginationAsync(page, cancellationToken);
            return TransferActionResult(result);
        }
        return BadRequest(); 
    }

    [HttpGet("latest")]
    public async Task<ActionResult<UserTransferGridDTO>> GetLatestRecordsAsync(CancellationToken cancellationToken) 
    {
        var result = await _service.GetLatestRecordsAsync(cancellationToken);
        return TransferActionResult(result);
    }

    /// <summary>
    /// Get record by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserTransferDTO>> GetRecordById(int id, CancellationToken cancellationToken) 
    {
        if ( id.IsNotZero() )  
        {
            var result = await _service.GetRecordByIdAsync(id, cancellationToken);
            return TransferActionResult(result);
        }
        return BadRequest(); 
    }



    /// <summary>
    /// Get record by username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [HttpGet("by-username/{username}")]
    public async Task<ActionResult<UserTransferDTO>> GetUsernameAsync(string username, CancellationToken cancellationToken) 
    {
        if ( !string.IsNullOrEmpty(username) )  
        {
            var result = await _service.GetUsernameAsync(username, cancellationToken);
            return TransferActionResult(result);
        }
        return BadRequest(); 
    }

    /// <summary>
    /// Create a new User record
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<ActionResult<TransferDTO>> CreateRecord(CreateUserDTO dto, CancellationToken cancellationToken) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.CreateRecordAsync(dto, cancellationToken);
        return TransferActionResult(result);
    }

    /// <summary>
    /// Update an existing User record
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("update")]
    public async Task<ActionResult<TransferDTO>> UpdateRecord(UpdateUserDTO dto, CancellationToken cancellationToken) 
    {
        if (!ModelState.IsValid) { return BadRequest(); }
        var result = await _service.UpdateRecordAsync(dto, cancellationToken);
        return TransferActionResult(result);
    }

    /// <summary>
    /// Delete a User record by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<bool>> DeleteRecord(int id, CancellationToken cancellationToken) 
    {
        if (id.IsNotZero()) 
        {
            var result = await _service.DeleteRecordByIdAsync(id, cancellationToken);
            return result.IsNotNull() ? Ok(result) : BadRequest();
        }
        return BadRequest();
    }
}
