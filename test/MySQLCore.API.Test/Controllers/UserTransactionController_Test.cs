using AutoFixture;
using MySQLCore.Core.Interfaces.Services.User;
using MySQLCore.Core.Models.DTOs.User;

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
}