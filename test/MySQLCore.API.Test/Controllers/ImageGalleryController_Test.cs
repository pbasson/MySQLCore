using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using MySQLCore.API.Controllers;
using MySQLCore.Core.Interfaces.InterfaceServices;
using MySQLCore.Core.Models.DTOs.ImageDTOs;
using MySQLCore.Core.Test.Helpers;

namespace MySQLCore.API.Test.Controllers
{
    public class ImageGalleryController_Test : Base_Test
    {
        private readonly Mock<IImageTransactionService> _service = new();
        private readonly ImageTransactionController _controller;
        private readonly ILogger<ImageTransactionController> _logger = default;
        
        public ImageGalleryController_Test()
        {
            _controller = new ImageTransactionController(_service.Object,_logger);    
        }
        
        [Fact]
        public async Task GetAllRecords_CheckIsValue() {
            var response = _fixture.Create<List<ImageTransactionDTO>>();
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
            var response = new List<ImageTransactionDTO>();
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
            var response = _fixture.Create<List<ImageTransactionDTO>>();
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
            var response = new List<ImageTransactionDTO>();
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
            var response = _fixture.Create<ImageTransactionDTO>();
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
            var response = new ImageTransactionDTO();
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
            var parameter = _fixture.Create<CreateImageTransactionDTO>();

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
            var parameter = new CreateImageTransactionDTO ();
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
            var parameter = new UpdateImageTransactionDTO { ImageTransactionID = 1, ImageType = "John Doe"};
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
            var parameter = new UpdateImageTransactionDTO ();
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
}