using AntivirusProgram.Entities.Exceptions;
using AntivirusProgram.Entities.Models;
using AntivirusProgram.Services;
using AntivirusProgram.Services.Abstracts;
using AntivirusProgram.Services.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

        [HttpGet("{hash}")]
        public async Task<IActionResult> GetScanResultByHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return BadRequest("Hash değeri boş olamaz.");

            var result = await _service.FileHashRecordService.GetOrCreateScanResultByHashAsync(hash, trackChanges: false);
            if (result == null)
                throw new NotFoundException("Dosya tarama sonucu bulunamadı.");
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateVirus(string hash, string? fileName = null)
        {
            return Ok("Test deneme");
        }

    }
}
