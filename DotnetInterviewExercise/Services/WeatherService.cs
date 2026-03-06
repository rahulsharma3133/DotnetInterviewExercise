using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotnetInterviewExercise.Services
{
    public class WeatherService : IWeatherService
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetActiveAlertsStatusAsync()
        {
            var uri = $"{_configuration["API:WeatherBaseUrl"]}/alerts/active/count";
            _logger.LogInformation("Fetching active alerts from {Uri}", uri);

            try
            {
                var client = _httpClientFactory.CreateClient("WeatherApi");
                
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("User-Agent", _configuration["API:UserAgent"]);
                
                var response = await client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("Active alerts check successful");
                    return "Active Alerts OK";
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Active alerts check returned status {StatusCode}", response.StatusCode);
                return System.Net.WebUtility.HtmlEncode(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed for {Uri}", uri);
                return $"Service unavailable: {System.Net.WebUtility.HtmlEncode(ex.Message)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching active alerts from {Uri}", uri);
                return $"Error: {System.Net.WebUtility.HtmlEncode(ex.Message)}";
            }
        }
    }
}