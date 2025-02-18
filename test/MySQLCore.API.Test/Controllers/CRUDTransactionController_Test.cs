using Microsoft.Extensions.Logging;
using MySQLCore.API.Controllers;
using MySQLCore.Core.Interfaces.InterfaceControllers.Test;
using MySQLCore.Core.Interfaces.InterfaceServices;
using MySQLCore.Core.Models.DTOs;
using MySQLCore.Core.Test.Helpers;

namespace MySQLCore.API.Test.Controllers;

public class CRUDTransactionController_Test : Base_Test, ICRUDTransactionController_Test
{
    private readonly Mock<ICRUDTransactionService> _service = new Mock<ICRUDTransactionService>();
    private readonly CRUDTransactionController _controller;
    private readonly ILogger<CRUDTransactionController> _logger = default!;

    public CRUDTransactionController_Test()
    {
        _controller = new CRUDTransactionController(_service.Object, _logger);
    }

    [Fact]
    public async Task GetAllRecords_CheckIsValue() {
        var response = _fixture.Create<List<CRUDTransactionDTO>>();
        var request = _service.Setup(x => x.GetAllRecordsAsync() ).ReturnsAsync(response);
        try {
            var result = await _controller.GetAllRecords();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }
    
    [Fact]
    public async Task GetAllRecords_CheckValueEmpty() {
        var response = new List<CRUDTransactionDTO>();
        var request = _service.Setup(x => x.GetAllRecordsAsync() ).ReturnsAsync(response);
        try {
            var result = await _controller.GetAllRecords();

            Assert.NotNull(result);
            Assert.Empty(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task GetAllRecordsPagination_CheckIsValue() {
        var response = _fixture.Create<List<CRUDTransactionDTO>>();
        var parameter = _fixture.Create<int>();
        var request = _service.Setup(x => x.GetAllRecordsPaginationAsync(parameter)).ReturnsAsync(response);

        try {
            var result = await _controller.GetAllRecordsPaginationAsync(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }
    
    [Fact]
    public async Task GetAllRecordsPagination_CheckValueEmpty() {
        var response = new List<CRUDTransactionDTO>();
        var parameter = 0;
        var request = _service.Setup(x => x.GetAllRecordsPaginationAsync(parameter)).ReturnsAsync(response);

        try {
            var result = await _controller.GetAllRecordsPaginationAsync(parameter);

            Assert.NotNull(result);
            Assert.Null(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task GetRecordById_CheckIsValue() {
        var response = _fixture.Create<CRUDTransactionDTO>();
        var parameter = _fixture.Create<int>();
        var request = _service.Setup(x => x.GetRecordByIdAsync(parameter)).ReturnsAsync(response);

        try {
            var result = await _controller.GetRecordById(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task GetRecordById_CheckValueEmpty() {
        var response = new CRUDTransactionDTO();
        var parameter = 0;
        var request = _service.Setup(x => x.GetRecordByIdAsync(parameter)).ReturnsAsync(response);

        try {
            var result = await _controller.GetRecordById(parameter);

            Assert.NotNull(result);
            Assert.Null(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task CreateRecord_CheckIsValue() {
        var response = true;
        var parameter = new CreateCRUDTransactionDTO { Name = "John Doe"};
        var request = _service.Setup( x => x.CreateRecordAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _controller.CreateRecord(parameter);
            Assert.True(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task CreateRecord_CheckIsValueFalse() {
        var response = false;
        var parameter = new CreateCRUDTransactionDTO ();
        var request = _service.Setup( x => x.CreateRecordAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _controller.CreateRecord(parameter);
            Assert.False(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }


    [Fact]
    public async Task UpdateRecord_CheckIsValue() {
        var response = true;
        var parameter = new UpdateCRUDTransactionDTO { Id = 1, Name = "John Doe"};
        var request = _service.Setup( x => x.UpdateRecordAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _controller.UpdateRecord(parameter);
            Assert.True(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task UpdateRecord_CheckIsValueFalse() {
        var response = false;
        var parameter = new UpdateCRUDTransactionDTO ();
        var request = _service.Setup( x => x.UpdateRecordAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _controller.UpdateRecord(parameter);
            Assert.False(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task DeleteRecord_CheckIsValue() {
        var response = true;
        var parameter = 1;
        var request = _service.Setup( x => x.DeleteRecordByIdAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _controller.DeleteRecord(parameter);
            Assert.True(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task DeleteRecord_CheckIsValueFalse() {
        var response = false;
        var parameter = 0;
        var request = _service.Setup( x => x.DeleteRecordByIdAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _controller.DeleteRecord(parameter);
            Assert.False(result.Value);
        }
        catch (Exception) {
            throw;
        }
    }
}