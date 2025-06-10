using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AntivirusProgram.Core
{
    public class HashApiClient
    {
        private readonly string baseUrl;
        private readonly HttpClient httpClient;

        public HashApiClient(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://antivirusprogram-dsacfqcpa8h0fgbr.swedencentral-01.azurewebsites.net/swagger/index.html");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.Add("Cookie",
                "ARRAffinity=22a7daa836b64a8ce56c907737553d08297ff2e76cd06a1f52c29956b9a85c17; " +
                "ARRAffinitySameSite=22a7daa836b64a8ce56c907737553d08297ff2e76cd06a1f52c29956b9a85c17");
        }

        public async Task<string> QueryHashAsync(string hash)
        {
            var response = await httpClient.GetAsync($"{baseUrl}/api/Scan/{hash}");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> SubmitHashAsync(string hash, string fileName)
        {
            string endpoint = $"{baseUrl}/api/Scan?hash={Uri.EscapeDataString(hash)}&fileName={Uri.EscapeDataString(fileName)}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent("") // Boş içerik
            };
            request.Content.Headers.ContentLength = 0;

            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<(int StatusCode, string Content)> QueryHashWithStatusAsync(string hash)
        {
            var response = await httpClient.GetAsync($"{baseUrl}/api/Scan/{hash}");
            var content = await response.Content.ReadAsStringAsync();
            return ((int)response.StatusCode, content);
        }
    }
}
