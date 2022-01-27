using DotnetInterviewExercise.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Net;

namespace DotnetInterviewExercise.UnitTests.Controllers
{
    [TestFixture]
    public class HealthControllerTests
    {
        [Test]
        public void TestHealthPing()
        {
            var controller = new HealthController(null, null);

            var result = controller.Ping();

            var objectResult = result as ObjectResult;

            Assert.That(objectResult.StatusCode, Is.EqualTo((int) HttpStatusCode.OK));
            Assert.That(objectResult.Value.ToString().Contains("Pong"), Is.True);
        }
    }
}
