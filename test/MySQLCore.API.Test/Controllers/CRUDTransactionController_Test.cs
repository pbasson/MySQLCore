using MySQLCore.API.Controllers;
using MySQLCore.Core.Interfaces.InterfaceControllers.Test;
using MySQLCore.Core.Interfaces.InterfaceServices;
using MySQLCore.Core.Models.DTOs;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;

namespace MySQLCore.API.Test.Controllers;

public class CRUDTransactionController_Test : ICRUDTransactionController_Test
{
    private readonly IFixture _fixture;
    private readonly Mock<ICRUDTransactionService> _service;
    private readonly CRUDTransactionController _controller;
    private readonly ILogger<CRUDTransactionController> _logger = default!;

    public CRUDTransactionController_Test()
    {
        _fixture = new Fixture();
        _service = new Mock<ICRUDTransactionService>();
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
}