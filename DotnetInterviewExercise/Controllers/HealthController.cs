using DotnetInterviewExercise.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DotnetInterviewExercise.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IWeatherService weatherService, ILogger<HealthController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        /// <summary>
        /// GET Health
        /// </summary>
        /// <remarks>
        /// Publicly available API that returns Pong response to notify service is up.
        /// </remarks>
        /// <returns>Pong</returns>
        /// <response code="200">Service is up</response>
        /// <response code="500">Service is down</response>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            _logger.LogInformation("Ping endpoint called");
            return Ok("Pong");
        }

        /// <summary>
        /// GET Active Alerts Health
        /// </summary>
        /// <remarks>
        /// Publicly available API that returns "Active Alerts OK" response if the weather.gov service is up.
        /// </remarks>
        /// <returns>Active Alerts OK</returns>
        /// <response code="200">Service is up</response>
        /// <response code="500">Service is down</response>
        [HttpGet("alerts")]
        public async Task<IActionResult> ActiveAlerts()
        {
            _logger.LogInformation("Active alerts endpoint called");
            var result = await _weatherService.GetActiveAlertsStatusAsync();
            return Ok(result);
        }
    }
}