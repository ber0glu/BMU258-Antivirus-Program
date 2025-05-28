using AntivirusProgram.Services.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntivirusProgram.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ScanController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("I love you Ömer");
        }
    }
}
