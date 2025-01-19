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
    public class ItemGroupIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private string ItemUrl = "/api/v2/items";
        private string ItemLineUrl = "/api/v2/itemlines";
        private string ItemTypeUrl = "/api/v2/itemtypes";
        private string ItemGroupUrl = "/api/v2/item_groups";
        private string SupplierUrl = "/api/v2/suppliers";
        private ItemType[] testItemTypes = [
                new ItemType(){Id = 1, Name = "type 1", Description = "Description of itemType 1"}
            ];
        
        private ItemLine[] testItemLines = [
                new ItemLine(){Id = 1, Name = "line 1", Description = "Description of itemLine 1"},
                new ItemLine(){Id = 2, Name = "line 2", Description = "Description of itemLine 2"}
            ];
        
        private ItemGroup[] testItemGroups = [
                new ItemGroup(){Id = 1, Name = "group 1", Description = "Description of itemgroup 1"}
            ];
        
        private Supplier[] testSuppliers = [
                new Supplier(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", AddressExtra = "Boven", ZipCode = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactName = "Fem Keijzer", PhoneNumber = "(078) 0013363", Reference = ":)"},
            ];
        
        private Item[] testItems = [
                new Item(){Uid = "P00001", ItemType=1, ItemLine=1, ItemGroup=1, SupplierId=1}
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
            addTestItemGroupsToDB(client);
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

        private void addTestItemGroupsToDB(HttpClient client)
        {
            // Add both Suppliers to db
            foreach(ItemGroup itemGroup in testItemGroups)
            {
                string jsonData = JsonConvert.SerializeObject(itemGroup);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync($"{ItemGroupUrl}", postContent).GetAwaiter().GetResult();
            }
        }

        private void addTestSuppliersToDB(HttpClient client)
        {
            // Add both Suppliers to db
            foreach(Supplier supplier in testSuppliers)
            {
                string jsonData = JsonConvert.SerializeObject(supplier);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync($"{SupplierUrl}", postContent).GetAwaiter().GetResult();
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
        public async Task test_get_all()
        {
            // Act
            var response = await client.GetAsync(ItemGroupUrl);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string responseData = await response.Content.ReadAsStringAsync();
            var itemGroups = JsonConvert.DeserializeObject<List<ItemGroup>>(responseData);

            Assert.IsNotNull(itemGroups);
            Assert.AreEqual(testItemGroups.Length, itemGroups.Count);
            Assert.AreEqual(testItemGroups[0].Name, itemGroups[0].Name);
        }

        [TestMethod]
        public async Task test_get_one()
        {
            // Arrange
            int testId = 1;

            // Act
            var response = await client.GetAsync($"{ItemGroupUrl}/{testId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string responseData = await response.Content.ReadAsStringAsync();
            var itemGroup = JsonConvert.DeserializeObject<ItemGroup>(responseData);

            Assert.IsNotNull(itemGroup);
            Assert.AreEqual(testId, itemGroup.Id);
            Assert.AreEqual(testItemGroups[0].Name, itemGroup.Name);
        }

        [TestMethod]
        public async Task test_post()
        {
            // Arrange
            var newItemGroup = new ItemGroup
            {
                Name = "New Group",
                Description = "Description of new group"
            };
            string jsonData = JsonConvert.SerializeObject(newItemGroup);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(ItemGroupUrl, postContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string responseData = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseData.Contains("Added itemGroup"));
        }

        [TestMethod]
        public async Task test_put()
        {
            // Arrange
            int testId = 1;
            var updatedItemGroup = new ItemGroup
            {
                Id = testId,
                Name = "Updated Group",
                Description = "Updated Description"
            };
            string jsonData = JsonConvert.SerializeObject(updatedItemGroup);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"{ItemGroupUrl}/{testId}", putContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string responseData = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseData.Contains("Updated"));
        }

        [TestMethod]
        public async Task test_delete_itemGroup()
        {
            // Arrange
            int testId = 1;

            // Act
            var response = await client.DeleteAsync($"{ItemGroupUrl}/{testId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string responseData = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseData.Contains("Deleted itemGroup"));

            // Verify it no longer exists
            var getResponse = await client.GetAsync($"{ItemGroupUrl}/{testId}");
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
        

    }
}