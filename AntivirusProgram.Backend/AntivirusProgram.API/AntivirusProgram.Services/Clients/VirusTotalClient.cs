using AntivirusProgram.Entities.Exceptions;
using AntivirusProgram.Entities.Models;
using AntivirusProgram.Services.Abstracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntivirusProgram.Services.Clients
{
    public class VirusTotalClient : IVirusTotalClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public VirusTotalClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = configuration["VirusTotal:ApiKey"]
                      ?? throw new InvalidOperationException("VirusTotal API key not configured.");
        }

        public async Task<VirusTotalResult> QueryHashAsync(string sha256)
        {
            if (string.IsNullOrWhiteSpace(sha256))
                throw new ArgumentException("SHA256 value must not be null or empty.", nameof(sha256));

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.virustotal.com/api/v3/files/{sha256}");
            request.Headers.Add("x-apikey", _apiKey);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new NotAVirusException("Virus Bulunamadı");
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            string fileName = "unknown";
            try
            {
                using var document = JsonDocument.Parse(jsonString);
                fileName = document.RootElement
                    .GetProperty("data")
                    .GetProperty("attributes")
                    .GetProperty("names")[0]
                    .GetString() ?? "unknown";
            }
            catch
            {
            }

            return new VirusTotalResult
            {
                FileName = fileName,
                JsonResult = jsonString
            };
        }
    }
}
