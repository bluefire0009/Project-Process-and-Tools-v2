using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using CargoHub.HelperFuctions;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ItemIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private string ItemUrl = "/api/v2/items";
        private string ItemLineUrl = "/api/v2/itemlines";
        private string ItemTypeUrl = "/api/v2/itemtypes";
        private string ItemGroupUrl = "/api/v2/item_groups";
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
            addTestItemsToDB(client);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();  // Optionally delete the database
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
            var response = client.GetAsync(ItemUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItems = JsonConvert.DeserializeObject<Item[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testItems.Length, resultItems.Length);
            
            //Clear the date fields so I can just assert using .equals function
            resultItems.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItems.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});

            for(int itemIterator = 0; itemIterator<resultItems.Length; itemIterator++)
            {
                Assert.AreEqual(testItems[itemIterator], resultItems[itemIterator]);
            }
        }

        [TestMethod]
        public void test_get_one()
        {
            // Arrange

            // Act
            var response = client.GetAsync(ItemUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItems = JsonConvert.DeserializeObject<Item[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testItems.Length, resultItems.Length);
            
            //Clear the date fields so I can just assert using .equals function
            resultItems.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItems.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});

            for(int itemIterator = 0; itemIterator<resultItems.Length; itemIterator++)
            {
                Assert.AreEqual(testItems[itemIterator], resultItems[itemIterator]);
            }
        }
    }
}