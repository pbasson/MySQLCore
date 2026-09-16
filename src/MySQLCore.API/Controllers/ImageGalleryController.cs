namespace MySQLCore.API.Controllers;

[Route("api/image-gallery")]
[ApiController]
public class ImageGalleryController : BaseController
{
    private readonly IImageGalleryService _service;
    public ImageGalleryController(IImageGalleryService service, ILogger<ImageGalleryController> logger) : base(logger) {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    public async Task<ActionResult<TransferImageGalleryGridDTO>> GetAllRecordsAsync(CancellationToken cancellationToken) 
    {
        var result = await _service.GetAllRecordsAsync(cancellationToken);
        return result.IsNotNull() && result.Records != null && result.Records.Count > 0 ? Ok(result) : NotFound();
    }

    [HttpGet("by-page/{page:int}")]
    public async Task<ActionResult<TransferImageGalleryGridDTO>> GetRecordsByPaginationAsync(int page, CancellationToken cancellationToken) 
    {
        if ( !page.IsNotZero() ) return BadRequest(); 

        var result = await _service.GetRecordsByPaginationAsync(page, cancellationToken);
        return result.IsNotNull() && result.Records != null && result.Records.Count > 0 ? Ok(result) : NotFound();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransferImageGalleryDTO>> GetRecordByIdAsync(int id, CancellationToken cancellationToken) 
    {
        if ( !id.IsNotZero() ) return BadRequest(); 

        var result = await _service.GetRecordByIdAsync(id, cancellationToken);
        return result.IsNotNull() && result.Record != null ? Ok(result) : NoContent();
    }

    [HttpGet("by-name/{galleryName}")]
    public async Task<ActionResult<TransferImageGalleryGridDTO>> GetRecordsByGalleryNameAsync(string galleryName, CancellationToken cancellationToken) 
    {
        if (galleryName.Length > 3) return BadRequest(); 
        var result = await _service.GetRecordsByGalleryNameAsync(galleryName, cancellationToken);
        return result.IsNotNull() && result.Records != null && result.Records.Count > 0 ? Ok(result) : NotFound();
    }

    [HttpPost("create")]
    public async Task<ActionResult<TransferDTO>> CreateRecord(CreateImageGalleryDTO dto, CancellationToken cancellationToken) 
    {
        var result = await _service.CreateRecordAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetRecordByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<TransferDTO>> UpdateRecord(UpdateImageGalleryDTO dto, CancellationToken cancellationToken) 
    {
        var result = await _service.UpdateRecordAsync(dto, cancellationToken);
        return result.IsNotNull() && result.Id > 0 ? Ok(result) : NotFound();
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<bool>> DeleteRecord(int id, CancellationToken cancellationToken) 
    {
        if ( !id.IsNotZero() ) return BadRequest(); 

        var result = await _service.DeleteRecordByIdAsync(id, cancellationToken);
        return result.IsNotNull() ? Ok(result) : BadRequest();
    }
}
