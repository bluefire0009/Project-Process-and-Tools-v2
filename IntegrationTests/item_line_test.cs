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
    public class ItemLineIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private string ItemUrl = "/api/v2/items";
        private string ItemLineUrl = "/api/v2/itemlines";
        private string ItemTypeUrl = "/api/v2/itemtypes";
        private string ItemGroupUrl = "/api/v2/item_groups";
        private ItemLine[] testItemLines = [
                new ItemLine(){Id = 1, Name = "line 1", Description = "Description of itemLine 1"},
                new ItemLine(){Id = 2, Name = "line 2", Description = "Description of itemLine 2"}
            ];
        
        private Item[] testItems = [
                new Item(){Uid = "P00001"},
                new Item(){Uid = "P00002"}
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
            addTestItemLinesToDB(client);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();  // Optionally delete the database
            }
        }

        private void addTestItemLinesToDB(HttpClient client)
        {
            // Add both Suppliers to db
            foreach(ItemLine itemLine in testItemLines)
            {
                string jsonData = JsonConvert.SerializeObject(itemLine);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync($"{ItemLineUrl}", postContent).GetAwaiter().GetResult();
            }
        }

        private void addTestItemsToDB(HttpClient client)
        {
            // Add both Suppliers to db
            foreach(Item item in testItems)
            {
                string jsonData = JsonConvert.SerializeObject(item);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var result = client.PostAsync($"{ItemUrl}", postContent).GetAwaiter().GetResult();
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, result.Content.ToString());
            }
        }

        [TestMethod]
        public void test_get_all()
        {
            // Arrange

            // Act
            var response = client.GetAsync(ItemLineUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItemLines = JsonConvert.DeserializeObject<ItemLine[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testItemLines.Length, resultItemLines.Length);
            
            //Clear the date fields so I can just assert using .equals function
            resultItemLines.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItemLines.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});

            for(int itemLineIterator = 0; itemLineIterator<resultItemLines.Length; itemLineIterator++)
            {
                Assert.IsTrue(testItemLines[itemLineIterator].Equals(resultItemLines[itemLineIterator]));
            }
        }

        [TestMethod]
        public void test_get_one()
        {
            // Arrange

            // Act
            var response = client.GetAsync(ItemLineUrl + '/' + testItemLines[0].Id).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItemLine = JsonConvert.DeserializeObject<ItemLine>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            //Clear the date fields so I can just assert using .equals function
            resultItemLine.CreatedAt = new(); resultItemLine.UpdatedAt = new();
            testItemLines.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});

            Assert.IsTrue(testItemLines[0].Equals(resultItemLine));
        }

        [TestMethod]
        public void test_post()
        {
            // Arrange
            ItemLine testItemLine = new(){Id = 3, Name = "line 3", Description = "Description of itemLine 3"};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testItemLine);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(ItemLineUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(ItemLineUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItemLines = JsonConvert.DeserializeObject<ItemLine[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, postStatus);
            Assert.IsTrue(resultItemLines.Length == testItemLines.Length + 1);
            
            //Clear the date fields so I can just assert using .equals function
            resultItemLines.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItemLine.CreatedAt = new();
            testItemLine.UpdatedAt = new();
            Assert.IsTrue(resultItemLines.Any(s=>s.Equals(testItemLine)));
        }

        [TestMethod]
        public void test_put()
        {
            // Arrange
            ItemLine testItemLine = new(){Id = testItemLines[0].Id, Name = "edited line 1", Description = "Description of edited itemLine 1"};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testItemLine);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //Capture the time around when the put request happens to check it later
            DateTime roughPutDate = CETDateTime.Now();
            HttpStatusCode putStatus = client.PutAsync($"{ItemLineUrl}/{testItemLine.Id}", putContent).Result.StatusCode;

            var response = client.GetAsync(ItemLineUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItemLines = JsonConvert.DeserializeObject<ItemLine[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putStatus);
            Assert.IsTrue(resultItemLines.Length == testItemLines.Length);
            
            //Check the modification date
            Assert.IsTrue(resultItemLines.First(s=>s.Id==testItemLine.Id).UpdatedAt - roughPutDate <= TimeSpan.FromSeconds(60));

            //Clear the date fields so I can just assert using .equals function
            resultItemLines.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItemLine.CreatedAt = new();
            testItemLine.UpdatedAt = new();
            Assert.IsTrue(resultItemLines.Any(s=>s.Equals(testItemLine)), "itemLine was not updated properly");
        }

        [TestMethod]
        public void test_delete_itemLine()
        {
            // Arrange

            // Act
            HttpStatusCode deleteStatus = client.DeleteAsync($"{ItemLineUrl}/{testItemLines[0].Id}").Result.StatusCode;

            var response = client.GetAsync(ItemLineUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultLines = JsonConvert.DeserializeObject<ItemLine[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.IsTrue(resultLines.Length == testItemLines.Length - 1);
            Assert.IsFalse(resultLines.Any(s=>s.Equals(testItemLines[0])));
        }

        public void test_get_items_by_itemLine() {
            
        }
    }
}