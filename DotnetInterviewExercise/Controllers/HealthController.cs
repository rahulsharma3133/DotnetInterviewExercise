using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotnetInterviewExercise.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HealthController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
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
            var uri = _configuration["API:WeatherBaseUrl"] + "/alerts/active/count";
            var serviceRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            serviceRequest.Headers.Add("User-Agent", _configuration["API:UserAgent"]);
            var response = await _httpClient.SendAsync(serviceRequest);

            return new ContentResult
            {
                Content = response.StatusCode == HttpStatusCode.OK
                    ? "Active Alerts OK"
                    : await response.Content.ReadAsStringAsync(),
                ContentType = "text/string",
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
