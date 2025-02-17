using AutoFixture;
using MySQLCore.Infrastructure.Entities.Tables;
using MySQLCore.Infrastructure.Repos;

namespace MySQLCore.Infrastructure.Test.Repositories;

public class CRUDTransactionRepo_Test : BaseRepo_Test 
//  ICRUDTransactionController_Test
{
    private readonly CRUDTransactionRepo _repo ;
    
    public CRUDTransactionRepo_Test()
    {
        _repo = new CRUDTransactionRepo(_dBContext,_mapper);
    }

    [Fact]
    public async Task GetAllRecords_CheckIsValue()
    {
        var fixture = _fixture.Create<List<CRUDTransaction>>();
        _dBContext.AddRange(fixture);
        _dBContext.SaveChanges();
        try {
            var result = await _repo.GetAllRecordsAsync();

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
        var fixture = new List<CRUDTransaction>();
        _dBContext.AddRange(fixture);
        _dBContext.SaveChanges();
        try {
            var result = await _repo.GetAllRecordsAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        catch (Exception) {
            throw;
        }
    }

//     [Fact]
//     public Task GetRecordById_CheckIsValue()
//     {
//         throw new NotImplementedException();
//     }

//     [Fact]
//     public Task GetRecordById_CheckValueEmpty()
//     {
//         throw new NotImplementedException();
//     }

//     [Fact]
//     public Task CreateRecord_CheckIsValue()
//     {
//         throw new NotImplementedException();
//     }

//     [Fact]
//     public Task CreateRecord_CheckIsValueFalse()
//     {
//         throw new NotImplementedException();
//     }

//     [Fact]
//     public Task UpdateRecord_CheckIsValue()
//     {
//         throw new NotImplementedException();
//     }

//     [Fact]
//     public Task UpdateRecord_CheckIsValueFalse()
//     {
//         throw new NotImplementedException();
//     }
    
//     [Fact]
//     public Task DeleteRecord_CheckIsValue()
//     {
//         throw new NotImplementedException();
//     }

//     [Fact]
//     public Task DeleteRecord_CheckIsValueFalse()
//     {
//         throw new NotImplementedException();
//     }
}