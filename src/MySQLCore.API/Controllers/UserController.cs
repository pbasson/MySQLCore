namespace MySQLCore.API.Controllers;

[Route("api/user")]
[ApiController]
public sealed class UserController : BaseController
{
    private readonly IUserService _service;
    public UserController(IUserService service, ILogger<UserController> logger) : base(logger) 
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Get records by pagination 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    [HttpGet("by-page/{page:int}")]
    public async Task<ActionResult<UserTransferGridDTO>> GetRecordsByPagination(int page, CancellationToken cancellationToken) 
    {
        if ( !page.IsNotZero() ) return BadRequest(); 

        var result = await _service.GetRecordsByPaginationAsync(page, cancellationToken);
        return result.IsNotNull() && result.TotalRecords > 0 ? Ok(result) : NoContent();
    }

    [HttpGet("latest")]
    public async Task<ActionResult<UserTransferGridDTO>> GetLatestRecordsAsync(CancellationToken cancellationToken) 
    {
        var result = await _service.GetLatestRecordsAsync(cancellationToken);
        return result.IsNotNull() && result.TotalRecords > 0 ? Ok(result) : NoContent();
    }

    /// <summary>
    /// Get record by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserTransferDTO>> GetRecordById(int id, CancellationToken cancellationToken) 
    {
        if ( !id.IsNotZero() ) return BadRequest(); 
        
        var result = await _service.GetRecordByIdAsync(id, cancellationToken);
        return result.IsNotNull() && result.CheckRecord() ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Get record by username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [HttpGet("by-username/{username}")]
    public async Task<ActionResult<UserTransferDTO>> GetUsernameAsync(string username, CancellationToken cancellationToken) 
    {
        if (string.IsNullOrEmpty(username)) return BadRequest(); 

        var result = await _service.GetUsernameAsync(username, cancellationToken);
        return result.IsNotNull() && result.CheckRecord() ? Ok(result) : NotFound();        
    }

    /// <summary>
    /// Create a new User record
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<ActionResult<TransferDTO>> CreateRecord(CreateUserDTO dto, CancellationToken cancellationToken) 
    {
        var result = await _service.CreateRecordAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetRecordById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing User record
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("update")]
    public async Task<ActionResult<TransferDTO>> UpdateRecord(UpdateUserDTO dto, CancellationToken cancellationToken) 
    {
        var result = await _service.UpdateRecordAsync(dto, cancellationToken);
        return result.IsNotNull() && result.Id > 0 ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Delete a User record by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<bool>> DeleteRecord(int id, CancellationToken cancellationToken) 
    {
        if ( !id.IsNotZero() ) return BadRequest(); 

        var result = await _service.DeleteRecordByIdAsync(id, cancellationToken);
        return result ? Ok(result) : BadRequest();
    }
}
