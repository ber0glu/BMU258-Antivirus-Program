using AntivirusProgram.Entities.Exceptions;
using AntivirusProgram.Entities.Models;
using AntivirusProgram.Repositories.Abstracts;
using AntivirusProgram.Services.Abstracts;
using AntivirusProgram.Services.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntivirusProgram.Services
{
    public class VirusScanService : IVirusScanService
    {
        private readonly IRepositoryManager repositoryManager;
        private readonly IVirusTotalClient virusTotalClient;

        public VirusScanService(IRepositoryManager repositoryManager, IVirusTotalClient virusTotalClient)
        {
            this.repositoryManager = repositoryManager;
            this.virusTotalClient = virusTotalClient;
        }

        public async Task<FileScanResult> GetOrCreateScanResultByHashAsync(string hash, bool trackChanges)
        {
            var existing = await repositoryManager.VirusRepository.GetFileScanResultByHashAsync(hash, trackChanges);
            if (existing != null)
                return existing;

            var vtResult = await virusTotalClient.QueryHashAsync(hash);

            bool isVirus = false;
            try
            {
                using var doc = JsonDocument.Parse(vtResult.JsonResult);
                if (doc.RootElement.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("attributes", out var attrs) &&
                    attrs.TryGetProperty("last_analysis_stats", out var stats) &&
                    stats.TryGetProperty("malicious", out var maliciousCount))
                {
                    isVirus = maliciousCount.GetInt32() > 0;
                }
            }
            catch
            {
            }

            if (!isVirus)
                throw new NotAVirusException(hash);

            var newRecord = new FileScanResult
            {
                FileHash = hash,
                FileName = vtResult.FileName ?? "unknown",
                AdditionalInfo = vtResult.JsonResult,
                ScanDate = DateTime.UtcNow,
                IsVirus = true,
                IsTestFile = false
            };
            
            repositoryManager.VirusRepository.CreateFileScanResult(newRecord);
            await repositoryManager.SaveAsync();

            return newRecord;
        }

        public async Task<FileScanResult> CreateVirusAsync(string hash, string? fileName = null)
        {
            var existing = await repositoryManager.VirusRepository.GetFileScanResultByHashAsync(hash, false);
            if (existing != null)
                throw new FileHashAlreadyExistsException(hash);

            var newRecord = new FileScanResult
            {
                FileHash = hash,
                FileName = fileName ?? "unknown",
                AdditionalInfo = "",
                ScanDate = DateTime.UtcNow,
                IsVirus = true,
                IsTestFile = false
            };

            repositoryManager.VirusRepository.CreateFileScanResult(newRecord);
            await repositoryManager.SaveAsync();
            return newRecord;
        }
    }
}
