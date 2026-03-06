using DotnetInterviewExercise.Controllers;
using DotnetInterviewExercise.Models;
using DotnetInterviewExercise.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DotnetInterviewExercise.UnitTests.Controllers
{
    [TestFixture]
    public class WeatherControllerTests
    {
        private Mock<IWeatherService> _mockWeatherService;
        private Mock<ILogger<WeatherController>> _mockLogger;
        private WeatherController _controller;

        [SetUp]
        public void Setup()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _mockLogger = new Mock<ILogger<WeatherController>>();
            _controller = new WeatherController(_mockWeatherService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetWeatherByStation_ReturnsOk_WhenStationExists()
        {
            var weatherData = new WeatherResponse
            {
                StationName = "Test Station",
                StationId = "TEST123",
                TemperatureCelsius = 20.5,
                Description = "Clear"
            };

            _mockWeatherService.Setup(s => s.GetWeatherByStationNameAsync("Test Station"))
                .ReturnsAsync(weatherData);

            var result = await _controller.GetWeatherByStation("Test Station");
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(weatherData));
        }

        [Test]
        public async Task GetWeatherByStation_ReturnsNotFound_WhenStationDoesNotExist()
        {
            _mockWeatherService.Setup(s => s.GetWeatherByStationNameAsync("NonExistent"))
                .ReturnsAsync((WeatherResponse)null);

            var result = await _controller.GetWeatherByStation("NonExistent");
            var notFoundResult = result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetWeatherByStation_ReturnsInternalServerError_WhenExceptionThrown()
        {
            _mockWeatherService.Setup(s => s.GetWeatherByStationNameAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetWeatherByStation("Test Station");
            var statusResult = result as ObjectResult;

            Assert.That(statusResult, Is.Not.Null);
            Assert.That(statusResult.StatusCode, Is.EqualTo(500));
        }
    }
}