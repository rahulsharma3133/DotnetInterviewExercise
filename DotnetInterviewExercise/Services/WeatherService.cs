using Microsoft.Extensions.Configuration;
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

        public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> GetActiveAlertsStatusAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("WeatherApi");
                var uri = $"{_configuration["API:WeatherBaseUrl"]}/alerts/active/count";
                
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("User-Agent", _configuration["API:UserAgent"]);
                
                var response = await client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return "Active Alerts OK";
                }

                var content = await response.Content.ReadAsStringAsync();
                return System.Net.WebUtility.HtmlEncode(content);
            }
            catch (HttpRequestException ex)
            {
                return $"Service unavailable: {System.Net.WebUtility.HtmlEncode(ex.Message)}";
            }
            catch (Exception ex)
            {
                return $"Error: {System.Net.WebUtility.HtmlEncode(ex.Message)}";
            }
        }
    }
}