using Microsoft.AspNetCore.Mvc;
using MySQLCore.Core.CoreHelpers;
using MySQLCore.Core.DTOs;
using MySQLCore.Core.Interfaces.InterfaceServices;

namespace MySQLCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CRUDTransactionController : ControllerBase
    {
        private readonly ICRUDTransactionService _service;
        public CRUDTransactionController(ICRUDTransactionService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("GetRecords")]
        public async Task<ActionResult<List<CRUDTransactionDTO>>> GetRecords(){
            var result = await _service.GetAllRecords();
            return result.NullChecker() && result.Any() ? result : new List<CRUDTransactionDTO>();
        }

        [HttpGet]
        [Route("GetRecordById/{Id:int}")]
        public async Task<ActionResult<CRUDTransactionDTO>> GetRecordById(int Id){
            var result = await _service.GetRecordById(Id);
            return result.NullChecker() ? result : new CRUDTransactionDTO();
        }

        [HttpPost]
        [Route("CreateRecord")]
        public async Task<ActionResult<bool>> CreateRecord(CRUDTransactionDTO dTO){
            var result = await _service.CreateRecord(dTO);
            return result;
        }

        [HttpPut]
        [Route("UpdateRecord")]
        public async Task<ActionResult<bool>> UpdateRecord(CRUDTransactionDTO dTO){
            var result = await _service.UpdateRecord(dTO);
            return result;
        }

        [HttpDelete]
        [Route("DeleteRecord")]
        public async Task<ActionResult<bool>> DeleteRecord(int id){
            var result = await _service.DeleteRecord(id);
            return result;
        }
    }
}
