using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestControllerIntegrationTests
    {

        [TestMethod]
        public async Task TestEndpoint_ReturnsOkWithExpectedContent()
        {
            // Arrange
            var application = new CargoHUbWebApplicationFactory();
            var client = application.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v2/Test");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("1", content);
        }
    }
}
