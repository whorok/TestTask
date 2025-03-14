
using Microsoft.Extensions.Options;

namespace TestTask.Services
{
    public class FNSApiService : IFNSApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FNSApiService> _logger;

        public FNSApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<FNSApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("FNSApi");
            _logger = logger;
        }
        public Task<string> GetCompanyInfoAsync(string inn)
        {
            throw new NotImplementedException();
        }
    }
}
