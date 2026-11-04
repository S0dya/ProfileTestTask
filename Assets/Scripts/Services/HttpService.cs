using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Configs;

namespace Services
{
    public class HttpService : IDisposable
    {
        private readonly HttpClient _httpClient;

        public HttpService(AppConfig appConfig)
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(appConfig.RequestTimeout)
            };
        }

        public async Task<byte[]> Download(string url, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var bytes = await response.Content.ReadAsByteArrayAsync();
            cancellationToken.ThrowIfCancellationRequested();
            return bytes;
        }
        
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
