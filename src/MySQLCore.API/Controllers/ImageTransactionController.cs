using Microsoft.AspNetCore.Mvc;
using MySQLCore.Core.CoreHelpers;
using MySQLCore.Core.Interfaces.InterfaceServices;
using MySQLCore.Core.Models.DTOs.ImageDTOs;

namespace MySQLCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageTransactionController : BaseController
    {
        private readonly IImageTransactionService _service;
        public ImageTransactionController(IImageTransactionService service, ILogger<CRUDTransactionController> logger) : base(logger) {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("GetAllRecords")]
        public async Task<ActionResult<List<ImageTransactionDTO>>> GetAllRecords() {
            var result = await _service.GetAllRecordsAsync();
            return result.NullChecker() && result.Count > 0 ? result : new List<ImageTransactionDTO>();
        }

        [HttpGet]
        [Route("GetAllRecordsPaginationAsync/{page:int}")]
        public async Task<ActionResult<List<ImageTransactionDTO>>> GetAllRecordsPaginationAsync(int page) {
            if (page.ZeroCheck())  {
                var result = await _service.GetAllRecordsPaginationAsync(page);
                return result.NullChecker() && result.Count > 0 ? result : new List<ImageTransactionDTO>();
            }
            
            return BadRequest(); 
        }


        [HttpGet]
        [Route("GetRecordById/{Id:int}")]
        public async Task<ActionResult<ImageTransactionDTO>> GetRecordById(int Id) {
            if (Id.ZeroCheck())  {
                var result = await _service.GetRecordByIdAsync(Id);
                return result.NullChecker() ? result : new ImageTransactionDTO();
            }
            
            return BadRequest(); 
        }

        [HttpPost]
        [Route("CreateRecord")]
        public async Task<ActionResult<bool>> CreateRecord(CreateImageTransactionDTO dTO) {
            if (!ModelState.IsValid) { return BadRequest(); }
            var result = await _service.CreateRecordAsync(dTO);
            return result;
        }

        [HttpPut]
        [Route("UpdateRecord")]
        public async Task<ActionResult<bool>> UpdateRecord(UpdateImageTransactionDTO dTO) {
            if (!ModelState.IsValid) { return BadRequest(); }
            var result = await _service.UpdateRecordAsync(dTO);
            return result;
        }

        [HttpDelete]
        [Route("DeleteRecord")]
        public async Task<ActionResult<bool>> DeleteRecord(int id) {
            if (id.ZeroCheck()) {
                var result = await _service.DeleteRecordByIdAsync(id);
                return result;
            }
            return BadRequest();
        }
    }
}
