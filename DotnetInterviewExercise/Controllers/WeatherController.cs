using DotnetInterviewExercise.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DotnetInterviewExercise.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        /// <summary>
        /// GET Weather by Station Name
        /// </summary>
        /// <param name="stationName">The name of the weather station</param>
        /// <returns>Latest weather observation for the station</returns>
        /// <response code="200">Weather data retrieved successfully</response>
        /// <response code="404">Station not found</response>
        /// <response code="500">Error retrieving weather data</response>
    
        [HttpGet("{stationName}")]
        public async Task<IActionResult> GetWeatherByStation(string stationName)
        {
            var sanitizedStationName = System.Net.WebUtility.HtmlEncode(stationName);
            _logger.LogInformation("Weather endpoint called for station: {StationName}", sanitizedStationName);

            try
            {
                var weatherData = await _weatherService.GetWeatherByStationNameAsync(stationName);

                if (weatherData == null)
                {
                    _logger.LogWarning("Station not found: {StationName}", sanitizedStationName);
                    return NotFound($"Station '{sanitizedStationName}' not found");
                }

                _logger.LogInformation("Weather data retrieved for station: {StationName}", sanitizedStationName);
                return Ok(weatherData);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather for station: {StationName}", sanitizedStationName);
                return StatusCode(500, "Error retrieving weather data");
            }
        }
    }
}
