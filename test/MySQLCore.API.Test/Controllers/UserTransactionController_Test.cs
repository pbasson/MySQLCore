namespace MySQLCore.API.Test.Controllers;

public class UserTransactionController_Test : Base_Test
{
    private readonly Mock<IUserService> _service = new();
    private readonly UserController _controller;
    private readonly ILogger<UserController> _logger = default!;

    public UserTransactionController_Test()
    {
        _controller = new UserController(_service.Object, _logger);
    }

    [Fact]
    public async Task GetAllRecordsPagination_ShouldReturnPopulated() 
    {
        // Arrange
        var response = new UserTransferGridDTO(
        [
            new() { Id = 1, UserName = "Johndoe", FirstName = "John", LastName = "Doe", Email = "johndoe@example.com" }, 
            new() { Id = 2, UserName = "Janedoe", FirstName = "Jane", LastName = "Doe", Email = "janedoe@example.com" }
        ]);

        var parameter = 10;
        var request = _service.Setup(x => x.GetRecordsByPaginationAsync(parameter, CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetRecordsByPagination(parameter, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<UserTransferGridDTO>(okResult.Value);

        Assert.NotEmpty(value.Records);
        Assert.Equal(2, value.TotalRecords);
    }
        
    [Fact]
    public async Task GetAllRecordsPagination_ShouldReturnBadRequest_WhenPageIsZero()
    {
        // Arrange
        var parameter = 0;

        // Act
        var result = await _controller.GetRecordsByPagination(parameter, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);

        _service.Verify( x => x.GetRecordsByPaginationAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllRecordsPagination_ShouldReturnNoContent_WhenNoRecordsFound()
    {
        // Arrange
        var response = new UserTransferGridDTO([]);
        var parameter = 1;

        _service.Setup(x => x.GetRecordsByPaginationAsync(parameter, CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetRecordsByPagination(parameter, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task GetLatestRecords_ShouldReturnOk_WhenRecordsExist()
    {
        // Arrange
        var response = new UserTransferGridDTO(
        [
            new() { Id = 1, UserName = "Johndoe", FirstName = "John", LastName = "Doe", Email = "johndoe@example.com" },
            new() { Id = 2, UserName = "Janedoe", FirstName = "Jane", LastName = "Doe", Email = "janedoe@example.com" }
        ]);

        _service.Setup(x => x.GetLatestRecordsAsync(CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetLatestRecordsAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var value = Assert.IsType<UserTransferGridDTO>(okResult.Value);

        Assert.NotEmpty(value.Records);
        Assert.Equal(2, value.TotalRecords);
    }

    [Fact]
    public async Task GetLatestRecords_ShouldReturnNoContent_WhenNoRecordsExist()
    {
        // Arrange
        var response = new UserTransferGridDTO([]);

        _service.Setup(x => x.GetLatestRecordsAsync(CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetLatestRecordsAsync(CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task GetRecordById_ShouldReturnBadRequest_WhenIdIsZero()
    {
        // Arrange
        var id = 0;

        // Act
        var result = await _controller.GetRecordById(id, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
        _service.Verify(x => x.GetRecordByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRecord_ShouldReturnCreatedAtAction_WhenCreated()
    {
        // Arrange
        var dto = new CreateUserDTO { UserName = "Johndoe", FirstName = "John", LastName = "Doe", Email = "johndoe@example.com"};
        var response = new TransferDTO(id: 1, message: "Success: Entity Created", serviceResultType: ServiceResultType.Success);

        _service.Setup(x => x.CreateRecordAsync(dto, CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.CreateRecord(dto, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(nameof(_controller.GetRecordById), createdResult.ActionName);
        Assert.Equal(response, createdResult.Value);
        Assert.Equal(1, createdResult.RouteValues!["id"]);

        _service.Verify(x => x.CreateRecordAsync(dto, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateRecord_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        var dto = new UpdateUserDTO { Id = 1, UserName = "Johndoe", FirstName = "John", LastName = "Doe", Email = "johndoe@example.com"};
        var response = new TransferDTO(id: 1, message: "Success: Entity Created", serviceResultType: ServiceResultType.Success);

        _service.Setup(x => x.UpdateRecordAsync(dto, CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.UpdateRecord(dto, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<TransferDTO>(okResult.Value);

        Assert.Equal(response, value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        _service.Verify(x => x.UpdateRecordAsync(dto, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateRecord_ShouldReturnNotFound_WhenUpdateFails()
    {
        // Arrange
        var dto = new UpdateUserDTO { Id = 1, UserName = "Johndoe", FirstName = "John", LastName = "Doe", Email = "johndoe@example.com"};
        var response = new TransferDTO(id: 0, message: "", serviceResultType: ServiceResultType.NotFound);

        _service.Setup(x => x.UpdateRecordAsync(dto, CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.UpdateRecord(dto, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);

        _service.Verify(x => x.UpdateRecordAsync(dto, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteRecord_ShouldReturnOk_WhenDeleteSucceeds()
    {
        // Arrange
        var id = 1;

        _service.Setup(x => x.DeleteRecordByIdAsync(id, CancellationToken.None)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteRecord(id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<bool>(okResult.Value);

        Assert.True(value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        _service.Verify(x => x.DeleteRecordByIdAsync(id, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteRecord_ShouldReturnOk_WhenDeleteFails()
    {
        // Arrange
        var id = 1;

        _service.Setup(x => x.DeleteRecordByIdAsync(id, CancellationToken.None)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteRecord(id, CancellationToken.None);

        // Assert
        var resultAssert = Assert.IsType<BadRequestResult>(result.Result);

        Assert.Equal(StatusCodes.Status400BadRequest, resultAssert.StatusCode);

        _service.Verify(x => x.DeleteRecordByIdAsync(id, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteRecord_ShouldReturnBadRequest_WhenIdIsZero()
    {
        // Arrange
        var id = 0;

        // Act
        var result = await _controller.DeleteRecord(id, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);

        _service.Verify(x => x.DeleteRecordByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
