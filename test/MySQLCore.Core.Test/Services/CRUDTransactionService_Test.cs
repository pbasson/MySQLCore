using MySQLCore.Core.Interfaces.InterfaceControllers.Test;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Models.DTOs;
using MySQLCore.Core.Services;
using MySQLCore.Core.Test.Helpers;

namespace MySQLCore.Core.Test.Services;

public class CRUDTransactionService_Test : Base_Test, ICRUDTransactionController_Test
{
    private readonly Mock<ICRUDTransactionRepo> _repo = new();
    private readonly CRUDTransactionService _service;

    public CRUDTransactionService_Test()
    {
        _service = new CRUDTransactionService(_repo.Object);
    }

    [Fact]
    public async Task GetAllRecords_CheckIsValue()
    {
        var response = _fixture.Create< List<CRUDTransactionDTO> >();
        var request = _repo.Setup(x => x.GetAllRecordsAsync()).ReturnsAsync(response);
        try {
            var result = await _service.GetAllRecordsAsync();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task GetAllRecords_CheckValueEmpty()
    {
        var response = new List<CRUDTransactionDTO>();
        var request = _repo.Setup(x => x.GetAllRecordsAsync()).ReturnsAsync(response);
        try {
            var result = await _service.GetAllRecordsAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task GetAllRecordsPagination_CheckIsValue()
    {
        var response = _fixture.Create<List<CRUDTransactionDTO>>();
        var parameter = _fixture.Create<int>();
        var request = _repo.Setup(x => x.GetAllRecordsPaginationAsync(parameter)).ReturnsAsync(response);

        try {
            var result = await _service.GetAllRecordsPaginationAsync(parameter);

            Assert.NotNull(result);
            Assert.NotEmpty(result );
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task GetAllRecordsPagination_CheckValueEmpty()
    {
        var response = new List<CRUDTransactionDTO>();
        var parameter = 0;
        var request = _repo.Setup(x => x.GetAllRecordsPaginationAsync(parameter)).ReturnsAsync(response);

        try {
            var result = await _service.GetAllRecordsPaginationAsync(parameter);

            Assert.NotNull(result);
            Assert.Empty(result );
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task GetRecordById_CheckIsValue()
    {
        var response = _fixture.Create<CRUDTransactionDTO>();
        var parameter = _fixture.Create<int>();
        var request = _repo.Setup(x => x.GetRecordByIdAsync(parameter)).ReturnsAsync(response);

        try {
            var result = await _service.GetRecordByIdAsync(parameter);

            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id );
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task GetRecordById_CheckValueEmpty()
    {
        var response = new CRUDTransactionDTO();
        var parameter = 0;
        var request = _repo.Setup(x => x.GetRecordByIdAsync(parameter)).ReturnsAsync(response);

        try {
            var result = await _service.GetRecordByIdAsync(parameter);

            Assert.NotNull(result);
            Assert.Equal(0, result.Id );
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task CreateRecord_CheckIsValue()
    {
        var response = true;
        var parameter = new CreateCRUDTransactionDTO { Name = "John Doe"};
        var request = _repo.Setup( x => x.CreateRecordAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _service.CreateRecordAsync(parameter);
            Assert.True(result);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task CreateRecord_CheckIsValueFalse()
    {
        var response = false;
        var parameter = new CreateCRUDTransactionDTO ();
        var request = _repo.Setup( x => x.CreateRecordAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _service.CreateRecordAsync(parameter);
            Assert.False(result);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task UpdateRecord_CheckIsValue()
    {
        var response = true;
        var parameter = new UpdateCRUDTransactionDTO { Id = 1, Name = "John Doe"};
        var request = _repo.Setup( x => x.UpdateRecordAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _service.UpdateRecordAsync(parameter);
            Assert.True(result);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task UpdateRecord_CheckIsValueFalse()
    {
        var response = false;
        var parameter = new UpdateCRUDTransactionDTO ();
        var request = _repo.Setup( x => x.UpdateRecordAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _service.UpdateRecordAsync(parameter);
            Assert.False(result);
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task DeleteRecord_CheckIsValue()
    {
        var response = true;
        var parameter = 1;
        var request = _repo.Setup( x => x.DeleteRecordByIdAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _service.DeleteRecordByIdAsync(parameter);
            Assert.True(result );
        }
        catch (Exception) {
            throw;
        }
    }

    [Fact]
    public async Task DeleteRecord_CheckIsValueFalse()
    {
        var response = false;
        var parameter = 0;
        var request = _repo.Setup( x => x.DeleteRecordByIdAsync(parameter)).ReturnsAsync(response);
        try {
            var result = await _service.DeleteRecordByIdAsync(parameter);
            Assert.False(result);
        }
        catch (Exception) {
            throw;
        }
    }
}