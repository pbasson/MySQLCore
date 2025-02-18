using MySQLCore.Core.Models.DTOs.ImageDTOs;
using MySQLCore.Infrastructure.Entities.Tables.ImageTables;
using MySQLCore.Infrastructure.Repos;

namespace MySQLCore.Infrastructure.Test.Repositories {
    public class ImageTransactionRepo_Test : BaseRepo_Test {
        private readonly ImageTransactionRepo _repo ;
    
        public ImageTransactionRepo_Test() {
            _repo = new ImageTransactionRepo(_dBContext,_mapper);
        }
        
        [Fact]
        public async Task GetAllRecords_CheckIsValue()
        {
            var fixture = _fixture.Create<List<ImageTransaction>>();
            base.AddRange(fixture);
            
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
        public async Task GetAllRecords_CheckValueEmpty() {
            var fixture = new List<ImageTransaction>();
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
        public async Task GetAllRecordsPagination_CheckIsValue() {
            var response = _fixture.Create<List<ImageTransaction>>();
            AddRange(response);

            var parameter = 1;

            try {
                var result = await _repo.GetAllRecordsPaginationAsync(parameter);

                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
            catch (Exception) {
                throw;
            }
        }

        [Fact]
        public async Task GetAllRecordsPagination_CheckValueEmpty() {
            var response = new List<ImageTransaction>();
            AddRange(response);
            var parameter = 0;

            try {
                var result = await _repo.GetAllRecordsPaginationAsync(parameter);

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
            var response = _fixture.Create<ImageTransaction>();
            Add(response);

            try
            {
                var result = await _repo.GetRecordByIdAsync(response.ImageTransactionID);

                Assert.NotNull(result);
                Assert.NotEqual(0, result.ImageTransactionID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Fact]
        public async Task GetRecordById_CheckValueEmpty() {
            var response = new ImageTransaction();
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
            var payload = new ImageTransaction { ImageType = "John Doe"};
            Add(payload);
            
            try {
                var result = await _repo.CreateRecordAsync(_mapper.Map<CreateImageTransactionDTO>(payload));
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
            var payload = new ImageTransaction { ImageType = "John Doe"};
            Add(payload);

            var parameter = new UpdateImageTransactionDTO { ImageTransactionID = payload.ImageTransactionID, ImageType = "Tony Joe"};
            
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
            var payload = new ImageTransaction { ImageType = "John Doe"};
            Add(payload);

            try {
                var result = await _repo.DeleteRecordByIdAsync(payload.ImageTransactionID);
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
}