using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DocksIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private static string DocksUrl = "/api/v2/docks";
        private static string LocationsUrl = "/api/v2/locations";
        
        private Dock[] testDocks = new[]
        {
            new Dock { Id = 1, LocationId = 1, isDeleted = false },
            new Dock { Id = 2, LocationId = 2, isDeleted = false }
        };

        private Location[] testLocations = new[]
        {
            new Location { Id = 1, Code = "LOC1", Name = "Location 1" },
            new Location { Id = 2, Code = "LOC2", Name = "Location 2" }
        };

        private HttpClient client;

        [TestInitialize]
        public void Setup()
        {
            var scope = Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            _dbContext.Database.EnsureDeleted();  
            _dbContext.Database.EnsureCreated();
            client = CreateClient();

           
            AddTestLocationsToDB(client, testLocations);

            
            AddTestDocksToDB(client, testDocks);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext?.Database.EnsureDeleted();
        }

        private static void AddTestLocationsToDB(HttpClient client, Location[] locations)
        {
            foreach (var location in locations)
            {
                var jsonData = JsonConvert.SerializeObject(location);
                var postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync(LocationsUrl, postContent).GetAwaiter().GetResult();
            }
        }

        private static void AddTestDocksToDB(HttpClient client, Dock[] docks)
        {
            foreach (var dock in docks)
            {
                var jsonData = JsonConvert.SerializeObject(dock);
                var postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync(DocksUrl, postContent).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public async Task Test_GetAllDocks()
        {
            var response = await client.GetAsync(DocksUrl);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var docks = JsonConvert.DeserializeObject<List<Dock>>(responseBody);

            Assert.IsNotNull(docks);
            Assert.AreEqual(testDocks.Length, docks.Count);
        }

        [TestMethod]
        public async Task Test_GetDockById()
        {
            // Arrange
            int idToRetrieve = 1;

            // Act
            var response = await client.GetAsync($"{DocksUrl}/{idToRetrieve}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var dock = JsonConvert.DeserializeObject<Dock>(responseBody);

            Assert.IsNotNull(dock);
            Assert.AreEqual(idToRetrieve, dock.Id);
        }

        [TestMethod]
        public async Task Test_PostDock()
        {
            // Arrange
            var newDock = new Dock
            {
                Id = 3,
                LocationId = 1, 
                isDeleted = false
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(newDock), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(DocksUrl, jsonContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var getResponse = await client.GetAsync($"{DocksUrl}/{newDock.Id}");
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var responseBody = await getResponse.Content.ReadAsStringAsync();
            var dock = JsonConvert.DeserializeObject<Dock>(responseBody);
            Assert.AreEqual(newDock.LocationId, dock.LocationId);
        }

       [TestMethod]
public async Task Test_SoftDeleteDock()
{
    // Arrange
    int idToDelete = 1;

    // Act -
    var deleteResponse = await client.DeleteAsync($"{DocksUrl}/{idToDelete}");

    // Assert -
    Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

    // Act 
    var getResponse = await client.GetAsync($"{DocksUrl}/{idToDelete}");

    // Assert 
    Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
}
        [TestMethod]
        public async Task Test_PaginateDocks()
        {
            var response = await client.GetAsync($"{DocksUrl}/pagination?offset=0&limit=1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var docks = JsonConvert.DeserializeObject<List<Dock>>(responseBody);

            Assert.IsNotNull(docks);
            Assert.AreEqual(1, docks.Count);  
        }
    }
}