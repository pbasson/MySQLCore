using MySQLCore.Core.Models.DTOs;
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
        base.AddRange(fixture);
        
        try
        {
            var result = await _repo.GetAllRecordsAsync();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
        catch (Exception)
        {
            throw;
        }
    }



    [Fact]
    public async Task GetAllRecords_CheckValueEmpty()
    {
        var fixture = new List<CRUDTransaction>();
        base.AddRange(fixture);

        try {
            var result = await _repo.GetAllRecordsAsync();

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
        var response = _fixture.Create<CRUDTransaction>();
        Add(response);

        var parameter = response.Id;

        try
        {
            var result = await _repo.GetRecordByIdAsync(parameter);

            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [Fact]
    public async Task GetRecordById_CheckValueEmpty() {
        var response = new CRUDTransaction();
        Add(response);
        var parameter = 0;

        try {
            var result = await _repo.GetRecordByIdAsync(parameter);

            Assert.Null(result);
        }
        catch (Exception) {
            throw;
        }
    }

        [Fact]
        public async Task CreateRecord_CheckIsValue()
        {
            var payload = new CRUDTransaction { Name = "John Doe"};
            Add(payload);
            
            try {
                var result = await _repo.CreateRecordAsync(_mapper.Map<CreateCRUDTransactionDTO>(payload));
                Assert.True(result);
            }
            catch (Exception) {
                throw;
            }
        }

        [Fact]
        public async Task CreateRecord_CheckIsValueFalse()
        {
            try {
                var result = await _repo.CreateRecordAsync(null);
                Assert.False(result);
            }
            catch (Exception) {
                throw;
            }
        }

        [Fact]
        public async Task UpdateRecord_CheckIsValue()
        {
            var payload = new CRUDTransaction { Name = "John Doe"};
            Add(payload);

            var parameter = new UpdateCRUDTransactionDTO { Id = payload.Id, Name = "Tony Joe"};
            
            try {
                var result = await _repo.UpdateRecordAsync(parameter);
                Assert.True(result);
            }
            catch (Exception) {
                throw;
            }
        }

        [Fact]
        public async Task UpdateRecord_CheckIsValueFalse()
        {
            try {
                var result = await _repo.UpdateRecordAsync(null);
                Assert.False(result);
            }
            catch (Exception) {
                throw;
            }
        }

        [Fact]
        public async Task DeleteRecord_CheckIsValue()
        {
            var payload = new CRUDTransaction { Name = "John Doe"};
            Add(payload);

            try {
                var result = await _repo.DeleteRecordByIdAsync(payload.Id);
                Assert.True( result );
            }
            catch (Exception) {
                throw;
            }
        }

        [Fact]
        public async Task DeleteRecord_CheckIsValueFalse()
        {
            try {
                var result = await _repo.DeleteRecordByIdAsync(0);
                Assert.False( result );
            }
            catch (Exception) {
                throw;
            }
        }
}