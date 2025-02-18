using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Models.DTOs.ImageDTOs;
using MySQLCore.Core.Services;
using MySQLCore.Core.Test.Helpers;

namespace MySQLCore.Core.Test.Services {
    public class ImageTransactionService_Test : Base_Test
    {
        private readonly Mock<IImageTransactionRepo> _repo = new();
        private readonly ImageTransactionService _service;

        public ImageTransactionService_Test()
        {
            _service = new ImageTransactionService(_repo.Object);
        }
        
        [Fact]
        public async Task GetAllRecords_CheckIsValue()
        {
            var response = _fixture.Create< List<ImageTransactionDTO> >();
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
            var response = new List<ImageTransactionDTO>();
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
            var response = _fixture.Create<List<ImageTransactionDTO>>();
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
            var response = new List<ImageTransactionDTO>();
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
            var response = _fixture.Create<ImageTransactionDTO>();
            var parameter = _fixture.Create<int>();
            var request = _repo.Setup(x => x.GetRecordByIdAsync(parameter)).ReturnsAsync(response);

            try {
                var result = await _service.GetRecordByIdAsync(parameter);

                Assert.NotNull(result);
                Assert.NotEqual(0, result.ImageTransactionID );
            }
            catch (Exception) {
                throw;
            }
        }

        [Fact]
        public async Task GetRecordById_CheckValueEmpty()
        {
            var response = new ImageTransactionDTO();
            var parameter = 0;
            var request = _repo.Setup(x => x.GetRecordByIdAsync(parameter)).ReturnsAsync(response);

            try {
                var result = await _service.GetRecordByIdAsync(parameter);

                Assert.NotNull(result);
                Assert.Equal(0, result.ImageTransactionID );
            }
            catch (Exception) {
                throw;
            }
        }

           [Fact]
        public async Task CreateRecord_CheckIsValue()
        {
            var response = true;
            var parameter = new CreateImageTransactionDTO { ImageType = "John Doe"};
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
            var parameter = new CreateImageTransactionDTO ();
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
            var parameter = new UpdateImageTransactionDTO { ImageTransactionID = 1, ImageType = "John Doe"};
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
            var parameter = new UpdateImageTransactionDTO ();
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
}