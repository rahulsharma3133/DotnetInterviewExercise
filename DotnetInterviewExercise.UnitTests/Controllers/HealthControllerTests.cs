using DotnetInterviewExercise.Controllers;
using DotnetInterviewExercise.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DotnetInterviewExercise.UnitTests.Controllers
{
    [TestFixture]
    public class HealthControllerTests
    {
        private Mock<IWeatherService> _mockWeatherService;
        private HealthController _controller;

        [SetUp]
        public void Setup()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _controller = new HealthController(_mockWeatherService.Object);
        }
        [Test]
        public void Ping_ReturnsOkWithPong()
        {
            var result = _controller.Ping();
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("Pong"));
        }

        [Test]
        public async Task ActiveAlerts_ReturnsOk_WhenServiceSucceeds()
        {
            _mockWeatherService.Setup(s => s.GetActiveAlertsStatusAsync())
                .ReturnsAsync("Active Alerts OK");

            var result = await _controller.ActiveAlerts();
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("Active Alerts OK"));
        }
    }
}
