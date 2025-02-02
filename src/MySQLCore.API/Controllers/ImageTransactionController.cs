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
            var result = await _service.GetAllRecords();
            return result.NullChecker() && result.Any() ? result : new List<ImageTransactionDTO>();
        }

        [HttpGet]
        [Route("GetRecordById/{Id:int}")]
        public async Task<ActionResult<ImageTransactionDTO>> GetRecordById(int Id) {
            if (Id > 0)  {
                var result = await _service.GetRecordById(Id);
                return result.NullChecker() ? result : new ImageTransactionDTO();
            }
            
            return BadRequest(); 
        }

        [HttpPost]
        [Route("CreateRecord")]
        public async Task<ActionResult<bool>> CreateRecord(ImageTransactionDTO dTO) {
            if (!ModelState.IsValid) { return BadRequest(); }
            var result = await _service.CreateRecord(dTO);
            return result;
        }

        [HttpPut]
        [Route("UpdateRecord")]
        public async Task<ActionResult<bool>> UpdateRecord(ImageTransactionDTO dTO) {
            if (!ModelState.IsValid) { return BadRequest(); }
            var result = await _service.UpdateRecord(dTO);
            return result;
        }

        [HttpDelete]
        [Route("DeleteRecord")]
        public async Task<ActionResult<bool>> DeleteRecord(int id) {
            if (id > 0) {
                var result = await _service.DeleteRecord(id);
                return result;
            }
            
            return BadRequest();
        }
    }
}
