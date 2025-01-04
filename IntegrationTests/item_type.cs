using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using CargoHub.HelperFuctions;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ItemTypeIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private string ItemTypeUrl = "/api/v2/itemtypes";
        private ItemType[] testItemTypes = [
                new ItemType(){Id = 1, Name = "type 1", Description = "Description of itemType 1"},
                new ItemType(){Id = 2, Name = "type 2", Description = "Description of itemType 1"}
            ];
        private HttpClient client;

        [TestInitialize]
        public void Setup()
        {
            // Get the service provider and create a new scope for each test
            var scope = Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            // Reset the database to ensure it is clean for each test
            _dbContext.Database.EnsureDeleted();  // Delete any existing database
            _dbContext.Database.EnsureCreated();  // Create a new fresh database
            client = CreateClient();
            addTestItemTypesToDB(client);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();  // Optionally delete the database
            }
        }

        private void addTestItemTypesToDB(HttpClient client)
        {
            // Add both Suppliers to db
            foreach(ItemType itemType in testItemTypes)
            {
                string jsonData = JsonConvert.SerializeObject(itemType);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync($"{ItemTypeUrl}", postContent).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void test_get_all()
        {
            // Arrange

            // Act
            var response = client.GetAsync(ItemTypeUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItemTypes = JsonConvert.DeserializeObject<ItemType[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testItemTypes.Length, resultItemTypes.Length);
            
            //Clear the date fields so I can just assert using .equals function
            resultItemTypes.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItemTypes.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});

            for(int itemTypeIterator = 0; itemTypeIterator<resultItemTypes.Length; itemTypeIterator++)
            {
                Assert.IsTrue(testItemTypes[itemTypeIterator].Equals(resultItemTypes[itemTypeIterator]));
            }
        }

        [TestMethod]
        public void test_get_one()
        {
            // Arrange

            // Act
            var response = client.GetAsync(ItemTypeUrl + '/' + testItemTypes[0].Id).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItemType = JsonConvert.DeserializeObject<ItemType>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            //Clear the date fields so I can just assert using .equals function
            resultItemType.CreatedAt = new(); resultItemType.UpdatedAt = new();
            testItemTypes.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});

            Assert.IsTrue(testItemTypes[0].Equals(resultItemType));
        }

        [TestMethod]
        public void test_post()
        {
            // Arrange
            ItemType testItemType = new(){Id = 3, Name = "type 3", Description = "Description of itemType 3"};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testItemType);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(ItemTypeUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(ItemTypeUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItemTypes = JsonConvert.DeserializeObject<ItemType[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, postStatus);
            Assert.IsTrue(resultItemTypes.Length == testItemTypes.Length + 1);
            
            //Clear the date fields so I can just assert using .equals function
            resultItemTypes.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItemType.CreatedAt = new();
            testItemType.UpdatedAt = new();
            Assert.IsTrue(resultItemTypes.Any(s=>s.Equals(testItemType)));
        }

        [TestMethod]
        public void test_put()
        {
            // Arrange
            ItemType testItemType = new(){Id = testItemTypes[0].Id, Name = "edited type 1", Description = "Description of edited itemType 1"};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testItemType);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //Capture the time around when the put request happens to check it later
            DateTime roughPutDate = CETDateTime.Now();
            HttpStatusCode putStatus = client.PutAsync($"{ItemTypeUrl}/{testItemType.Id}", putContent).Result.StatusCode;

            var response = client.GetAsync(ItemTypeUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItemTypes = JsonConvert.DeserializeObject<ItemType[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putStatus);
            Assert.IsTrue(resultItemTypes.Length == testItemTypes.Length);
            
            //Check the modification date
            Assert.IsTrue(resultItemTypes.First(s=>s.Id==testItemType.Id).UpdatedAt - roughPutDate <= TimeSpan.FromSeconds(60));

            //Clear the date fields so I can just assert using .equals function
            resultItemTypes.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItemType.CreatedAt = new();
            testItemType.UpdatedAt = new();
            Assert.IsTrue(resultItemTypes.Any(s=>s.Equals(testItemType)));
        }

        [TestMethod]
        public void test_delete_supplier()
        {
            // Arrange

            // Act
            HttpStatusCode deleteStatus = client.DeleteAsync($"{ItemTypeUrl}/{testItemTypes[0].Id}").Result.StatusCode;

            var response = client.GetAsync(ItemTypeUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTypes = JsonConvert.DeserializeObject<ItemType[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.IsTrue(resultTypes.Length == testItemTypes.Length - 1);
            Assert.IsFalse(resultTypes.Any(s=>s.Equals(testItemTypes[0])));
        }
    }
}