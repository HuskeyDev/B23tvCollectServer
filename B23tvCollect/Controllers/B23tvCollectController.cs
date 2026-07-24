using B23tvCollect.DataAccess.RocksDb;
using B23tvCollect.Models;
using B23tvCollect.Services;
using Microsoft.AspNetCore.Mvc;

namespace B23tvCollect.Controllers
{
    //[Route("")]
    [ApiController]
    public class B23tvCollectController : ControllerBase
    {
        private readonly Collect _collect;
        public B23tvCollectController(AppRocksDb rocksDb) { _collect = new Collect(rocksDb); }
        [HttpPost("b23tvRecord")]
        public IActionResult NewB23tvRecord([FromBody] LinkRequest.NewB23tv arg)
        {
            _collect.NewRecord(arg.b23tvCode, arg.target);
            return Ok();
        }
        [HttpGet("targets")]
        public LinkResponse.ReturnTarget GetTarget([FromQuery] LinkRequest.GetTarget arg)
        {
            return _collect.FindAllRecordsByB23(arg.b23tvCode);
        }
    }
}
