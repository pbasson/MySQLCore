namespace MySQLCore.API.Test.Controllers;

public class ImageGalleryController_Test : Base_Test
{
    private readonly Mock<IImageTransactionService> _service = new();
    private readonly ImageTransactionController _controller;
    private readonly ILogger<ImageTransactionController> _logger = default!;

    public ImageGalleryController_Test()
    {
        _controller = new ImageTransactionController(_service.Object, _logger);
    }

    [Fact]
    public async Task GetAllRecords_ReturnsOkWithRecords()
    {
        // Arrange
        var response = new List<ImageTransactionDTO>
        {
            new()
            {
                ImageTransactionID = 1,
                ImageType = "profile",
                ImageGalleries =
                [
                    new ImageGalleryDTO
                    {
                        ImageGalleryId = 10,
                        ImageTransactionID = 1,
                        ImagePath = "/images/profile.png"
                    }
                ]
            }
        };
        _service.Setup(x => x.GetAllRecordsAsync()).ReturnsAsync(response);

        // Act
        var result = await _controller.GetAllRecords();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<List<ImageTransactionDTO>>(okResult.Value);
        Assert.Same(response, value);
        Assert.NotEmpty(value);
        _service.Verify(x => x.GetAllRecordsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllRecords_ReturnsOkWithEmptyRecords()
    {
        // Arrange
        var response = new List<ImageTransactionDTO>();
        _service.Setup(x => x.GetAllRecordsAsync()).ReturnsAsync(response);

        // Act
        var result = await _controller.GetAllRecords();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<List<ImageTransactionDTO>>(okResult.Value);
        Assert.Same(response, value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task GetAllRecordsPagination_WithValidPage_ReturnsOkWithRecords()
    {
        // Arrange
        var response = new List<ImageTransactionDTO>
        {
            new() { ImageTransactionID = 1, ImageType = "gallery" }
        };
        const int page = 1;
        _service.Setup(x => x.GetAllRecordsPaginationAsync(page)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetAllRecordsPaginationAsync(page);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<List<ImageTransactionDTO>>(okResult.Value);
        Assert.Same(response, value);
        Assert.NotEmpty(value);
        _service.Verify(x => x.GetAllRecordsPaginationAsync(page), Times.Once);
    }

    [Fact]
    public async Task GetAllRecordsPagination_WithZeroPage_ReturnsBadRequest()
    {
        var result = await _controller.GetAllRecordsPaginationAsync(0);

        Assert.IsType<BadRequestResult>(result.Result);
        _service.Verify(x => x.GetAllRecordsPaginationAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetRecordById_WithExistingRecord_ReturnsRecord()
    {
        var response = new ImageTransactionDTO
        {
            ImageTransactionID = 1,
            ImageType = "profile"
        };
        const int id = 1;
        _service.Setup(x => x.GetRecordByIdAsync(id)).ReturnsAsync(response);

        var result = await _controller.GetRecordById(id);

        Assert.Same(response, result.Value);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task GetRecordById_WithMissingRecord_ReturnsNotFound()
    {
        var response = new ImageTransactionDTO();
        const int id = 1;
        _service.Setup(x => x.GetRecordByIdAsync(id)).ReturnsAsync(response);

        var result = await _controller.GetRecordById(id);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetRecordById_WithZeroId_ReturnsBadRequest()
    {
        var result = await _controller.GetRecordById(0);

        Assert.IsType<BadRequestResult>(result.Result);
        _service.Verify(x => x.GetRecordByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateRecord_WithSuccessfulTransfer_ReturnsOk()
    {
        var request = new CreateImageTransactionDTO
        {
            ImageType = "profile",
            ImageGalleries = [new CreateImageGalleryDTO { ImagePath = "/images/profile.png" }]
        };
        var response = new TransferDTO(1, actionStatusType: ActionStatusType.Ok);
        _service.Setup(x => x.CreateRecordAsync(request)).ReturnsAsync(response);

        var result = await _controller.CreateRecord(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task CreateRecord_WithCreatedTransfer_ReturnsCreatedStatus()
    {
        var request = new CreateImageTransactionDTO
        {
            ImageType = "banner",
            ImageGalleries = [new CreateImageGalleryDTO { ImagePath = "/images/banner.png" }]
        };
        var response = new TransferDTO(2, actionStatusType: ActionStatusType.Created);
        _service.Setup(x => x.CreateRecordAsync(request)).ReturnsAsync(response);

        var result = await _controller.CreateRecord(request);

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        Assert.Same(response, objectResult.Value);
    }

    [Fact]
    public async Task CreateRecord_WithInvalidModelState_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError(nameof(CreateImageTransactionDTO.ImageType), "Required");

        var result = await _controller.CreateRecord(new CreateImageTransactionDTO());

        Assert.IsType<BadRequestResult>(result.Result);
        _service.Verify(x => x.CreateRecordAsync(It.IsAny<CreateImageTransactionDTO>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRecord_WithSuccessfulTransfer_ReturnsOk()
    {
        var request = new UpdateImageTransactionDTO
        {
            ImageTransactionID = 1,
            ImageType = "updated",
            ImageGalleries =
            [
                new ImageGalleryDTO
                {
                    ImageGalleryId = 10,
                    ImageTransactionID = 1,
                    ImagePath = "/images/updated.png"
                }
            ]
        };
        var response = new TransferDTO(1, actionStatusType: ActionStatusType.Ok);
        _service.Setup(x => x.UpdateRecordAsync(request)).ReturnsAsync(response);

        var result = await _controller.UpdateRecord(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task UpdateRecord_WithNotFoundTransfer_ReturnsNotFound()
    {
        var request = new UpdateImageTransactionDTO { ImageTransactionID = 404, ImageType = "missing" };
        var response = TransferFactory.GetTransferFailure(TransferEnum.EntityNotExist);
        _service.Setup(x => x.UpdateRecordAsync(request)).ReturnsAsync(response);

        var result = await _controller.UpdateRecord(request);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Same(response, notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateRecord_WithInvalidModelState_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError(nameof(UpdateImageTransactionDTO.ImageTransactionID), "Required");

        var result = await _controller.UpdateRecord(new UpdateImageTransactionDTO());

        Assert.IsType<BadRequestResult>(result.Result);
        _service.Verify(x => x.UpdateRecordAsync(It.IsAny<UpdateImageTransactionDTO>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRecord_WithValidId_ReturnsOkWithResult()
    {
        const int id = 1;
        _service.Setup(x => x.DeleteRecordByIdAsync(id)).ReturnsAsync(true);

        var result = await _controller.DeleteRecord(id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.True(Assert.IsType<bool>(okResult.Value));
        _service.Verify(x => x.DeleteRecordByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteRecord_WhenServiceReturnsFalse_ReturnsOkWithFalse()
    {
        const int id = 1;
        _service.Setup(x => x.DeleteRecordByIdAsync(id)).ReturnsAsync(false);

        var result = await _controller.DeleteRecord(id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.False(Assert.IsType<bool>(okResult.Value));
    }

    [Fact]
    public async Task DeleteRecord_WithZeroId_ReturnsBadRequest()
    {
        var result = await _controller.DeleteRecord(0);

        Assert.IsType<BadRequestResult>(result.Result);
        _service.Verify(x => x.DeleteRecordByIdAsync(It.IsAny<int>()), Times.Never);
    }
}
