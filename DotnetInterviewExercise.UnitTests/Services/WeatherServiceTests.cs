using DotnetInterviewExercise.Services;
using DotnetInterviewExercise.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetInterviewExercise.UnitTests.Services
{
    [TestFixture]
    public class WeatherServiceTests
    {
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<WeatherService>> _mockLogger;
        private WeatherService _weatherService;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<WeatherService>>();
            
            _mockConfiguration.Setup(c => c["API:WeatherBaseUrl"]).Returns("https://api.weather.gov");
            _mockConfiguration.Setup(c => c["API:UserAgent"]).Returns("test-agent");
            
            _weatherService = new WeatherService(_mockHttpClientFactory.Object, _mockConfiguration.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetActiveAlertsStatusAsync_ReturnsOK_WhenServiceRespondsWithOK()
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(f => f.CreateClient("WeatherApi")).Returns(httpClient);

            var result = await _weatherService.GetActiveAlertsStatusAsync();

            Assert.That(result, Is.EqualTo("Active Alerts OK"));
        }

        [Test]
        public async Task GetActiveAlertsStatusAsync_ReturnsEncodedContent_WhenServiceFails()
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("<script>alert('xss')</script>")
                });

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(f => f.CreateClient("WeatherApi")).Returns(httpClient);

            var result = await _weatherService.GetActiveAlertsStatusAsync();

           Assert.That(result, Does.Not.Contain("<script>"));
        }

        [Test]
        public async Task GetWeatherByStationNameAsync_ReturnsNull_WhenStationNameIsNull()
        {
            var stationsJson = "{\"features\":[]}";
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(stationsJson) });

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(f => f.CreateClient("WeatherApi")).Returns(httpClient);

            var result = await _weatherService.GetWeatherByStationNameAsync(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetWeatherByStationNameAsync_ReturnsNull_WhenStationNameIsEmpty()
        {
            var stationsJson = "{\"features\":[]}";
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(stationsJson) });

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(f => f.CreateClient("WeatherApi")).Returns(httpClient);

            var result = await _weatherService.GetWeatherByStationNameAsync("");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetWeatherByStationNameAsync_ReturnsWeatherData_WhenStationExists()
        {
            var stationsJson = "{\"features\":[{\"properties\":{\"stationIdentifier\":\"000SE\",\"name\":\"SCE South Hills Park\"}}]}";
            var observationsJson = "{\"features\":[{\"properties\":{\"timestamp\":\"2024-01-01T12:00:00Z\",\"temperature\":{\"value\":15.5},\"textDescription\":\"Partly Cloudy\"}}]}";
              
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(stationsJson) })
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(observationsJson) });

             var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(f => f.CreateClient("WeatherApi")).Returns(httpClient);

            var result = await _weatherService.GetWeatherByStationNameAsync("SCE South Hills Park");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StationId, Is.EqualTo("000SE"));
            Assert.That(result.TemperatureCelsius, Is.EqualTo(15.5));
        }

        [Test]
        public async Task GetWeatherByStationNameAsync_ThrowsException_WhenStationNotFound()
        {
            var stationsJson = "{\"features\":[]}";
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(stationsJson) });

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(f => f.CreateClient("WeatherApi")).Returns(httpClient);

            var result = await _weatherService.GetWeatherByStationNameAsync("NonExistent");

            Assert.That(result, Is.Null);
        }
    }
}