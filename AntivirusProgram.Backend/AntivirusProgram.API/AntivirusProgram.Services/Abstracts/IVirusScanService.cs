using AntivirusProgram.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Services.Abstracts
{
    public interface IVirusScanService
    {
        Task<FileScanResult> GetOrCreateScanResultByHashAsync(string hash, bool trackChanges);
        Task<FileScanResult> CreateVirusAsync(string hash, string? fileName = null);
    }
}
