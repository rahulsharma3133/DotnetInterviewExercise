using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using DotnetInterviewExercise.Models;

namespace DotnetInterviewExercise.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        #region GetActiveAlertsStatusAsync
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
        #endregion GetActiveAlertsStatusAsync

        #region GetWeatherByStationNameAsync
        public async Task<WeatherResponse> GetWeatherByStationNameAsync(string stationName)
        {
            var stationId = await GetStationIdByNameAsync(stationName);
            if (string.IsNullOrEmpty(stationId))
            {
                return null;
            }

            return await GetLatestObservationAsync(stationId, stationName);
        }

        //Retrieves the station ID for a weather station based on its name
        private async Task<string> GetStationIdByNameAsync(string stationName)
        {
            var uri = $"{_configuration["API:WeatherBaseUrl"]}/stations?limit=100";
            _logger.LogInformation("Fetching stations from {Uri}", uri);

            var client = _httpClientFactory.CreateClient("WeatherApi");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("User-Agent", _configuration["API:UserAgent"]);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var stationList = JsonSerializer.Deserialize<StationListResponse>(content, options);

            var station = stationList?.Features?.FirstOrDefault(f => 
                f.Properties?.Name?.Equals(stationName, StringComparison.OrdinalIgnoreCase) == true);

            return station?.Properties?.StationIdentifier;
        }

        //Retrieves the latest weather observation for a specific weather station
        private async Task<WeatherResponse> GetLatestObservationAsync(string stationId, string stationName)
        {
            var uri = $"{_configuration["API:WeatherBaseUrl"]}/stations/{stationId}/observations?limit=1";
            _logger.LogInformation("Fetching observations from {Uri}", uri);

            var client = _httpClientFactory.CreateClient("WeatherApi");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("User-Agent", _configuration["API:UserAgent"]);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var observationResponse = JsonSerializer.Deserialize<ObservationResponse>(content, options);

            var observation = observationResponse?.Features?.FirstOrDefault()?.Properties;
            if (observation == null)
            {
                throw new Exception($"No observations found for station {stationId}");
            }

            return new WeatherResponse
            {
                StationName = stationName,
                StationId = stationId,
                Timestamp = observation.Timestamp,
                TemperatureCelsius = observation.Temperature?.Value,
                Description = observation.TextDescription,
                DewpointCelsius = observation.Dewpoint?.Value,
                WindSpeedKmh = observation.WindSpeed?.Value,
                WindDirectionDegrees = observation.WindDirection?.Value,
                BarometricPressurePa = observation.BarometricPressure?.Value,
                RelativeHumidityPercent = observation.RelativeHumidity?.Value
            };
        }
        #endregion GetWeatherByStationNameAsync
    }
}