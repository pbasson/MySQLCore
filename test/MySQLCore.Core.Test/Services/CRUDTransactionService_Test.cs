using AutoFixture;
using Moq;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Models.DTOs;
using MySQLCore.Core.Services;

namespace MySQLCore.Core.Test.Services;

public class CRUDTransactionService_Test 
// : ICRUDTransactionController_Test
{
    private readonly IFixture _fixture = new Fixture();
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

    // [Fact]
    // public Task CreateRecord_CheckIsValue()
    // {
    //     try {
            
    //     }
    //     catch (Exception) {
    //         throw;
    //     }
    // }

    // [Fact]
    // public Task CreateRecord_CheckIsValueFalse()
    // {
    //     try {
            
    //     }
    //     catch (Exception) {
    //         throw;
    //     }
    // }

    // [Fact]
    // public Task UpdateRecord_CheckIsValue()
    // {
    //     try {
            
    //     }
    //     catch (Exception) {
    //         throw;
    //     }
    // }

    // [Fact]
    // public Task UpdateRecord_CheckIsValueFalse()
    // {
    //     try {
            
    //     }
    //     catch (Exception) {
    //         throw;
    //     }
    // }

    // [Fact]
    // public Task DeleteRecord_CheckIsValue()
    // {
    //     try {
            
    //     }
    //     catch (Exception) {
    //         throw;
    //     }
    // }

    // [Fact]
    // public Task DeleteRecord_CheckIsValueFalse()
    // {
    //     try {
            
    //     }
    //     catch (Exception) {
    //         throw;
    //     }
    // }
}