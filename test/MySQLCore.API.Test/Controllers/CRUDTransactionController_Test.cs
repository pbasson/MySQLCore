using MySQLCore.API.Controllers;
using MySQLCore.Core.Interfaces.InterfaceServices;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using MySQLCore.Core.Models.DTOs;
using System.Threading.Tasks;

namespace MySQLCore.API.Test.Controllers;

public class CRUDTransactionController_Test
{
    private readonly IFixture _fixture;
    private readonly Mock<ICRUDTransactionService> _service;
    private readonly CRUDTransactionController _controller;
    private readonly ILogger<CRUDTransactionController> _logger;

    public CRUDTransactionController_Test()
    {
        _fixture = new Fixture();
        _service = new Mock<ICRUDTransactionService>();
        _controller = new CRUDTransactionController(_service.Object, _logger);
    }

    [Fact]
    public async Task GetAllRecords_CheckIsValue() {
        var response = _fixture.Create<List<CRUDTransactionDTO>>();
        var request = _service.Setup(x => x.GetAllRecords() ).ReturnsAsync(response);
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
    public async Task GetRecordById() {
        var response = _fixture.Create<CRUDTransactionDTO>();
        var parameter = _fixture.Create<int>();
        var request = _service.Setup(x => x.GetRecordById(parameter)).ReturnsAsync(response);

        try
        {
            var result = await _controller.GetRecordById(parameter);

            Assert.NotNull(result);
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}