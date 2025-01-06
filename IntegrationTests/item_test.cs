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
        private string SupplierUrl = "/api/v2/suppliers";
        private ItemType[] testItemTypes = [
                new ItemType(){Id = 1, Name = "type 1", Description = "Description of itemType 1"}
            ];
        
        private ItemLine[] testItemLines = [
                new ItemLine(){Id = 1, Name = "line 1", Description = "Description of itemLine 1"}
            ];
        
        private ItemGroup[] testItemGroups = [
                new ItemGroup(){Id = 1, Name = "group 1", Description = "Description of itemgroup 1"}
            ];
        
        private Supplier[] testSuppliers = [
                new Supplier(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", AddressExtra = "Boven", ZipCode = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactName = "Fem Keijzer", PhoneNumber = "(078) 0013363", Reference = ":)"},
            ];
        
        private Item[] testItems = [
                new Item(){Uid = "P00001", ItemType=1, ItemLine=1, ItemGroup=1, SupplierId=1},
                new Item(){Uid = "P00002", ItemType=1, ItemLine=1, ItemGroup=1, SupplierId=1}
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

            // test Item Line creation
            bool itemLineCreation;
            try {
                addTestItemLinesToDB(client);
                itemLineCreation = true;
            } catch {
                itemLineCreation = false;
            }

            Assert.IsTrue(itemLineCreation, "ItemLine creation failed, run itemline intergration tests for more information");
            
            // test item type creation
            bool itemTypeCreation;
            try {
                addTestItemTypesToDB(client);
                itemTypeCreation = true;
            } catch {
                itemTypeCreation = false;
            }

            Assert.IsTrue(itemTypeCreation, "ItemType creation failed, run ItemType intergration tests for more information");

            // test Item Group creation
            bool itemGroupCreation;
            try {
                addTestItemGroupsToDB(client);
                itemGroupCreation = true;
            } catch {
                itemGroupCreation = false;
            }

            Assert.IsTrue(itemGroupCreation, "ItemGroup creation failed, run itemgroup intergration tests for more information");

            // test Supplier creation
            bool SupplierCreation;
            try {
                addTestSuppliersToDB(client);
                SupplierCreation = true;
            } catch {
                SupplierCreation = false;
            }

            Assert.IsTrue(SupplierCreation, "Supplier creation failed, run Supplier intergration tests for more information");

            // test items creation
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

        [TestMethod]
        public void test_post()
        {
            // Arrange
            Item testItem = new Item(){Uid = "P00003", ItemType=1, ItemLine=1, ItemGroup=1, SupplierId=1};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testItem);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(ItemUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(ItemUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItems = JsonConvert.DeserializeObject<Item[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, postStatus);
            Assert.IsTrue(resultItems.Length == testItems.Length + 1);
            
            //Clear the date fields so I can just assert using .equals function
            resultItems.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItem.CreatedAt = new();
            testItem.UpdatedAt = new();
            Assert.IsTrue(resultItems.Any(s=>s.Equals(testItem)));
        }

        [TestMethod]
        public void test_put()
        {
            // Arrange
            Item testItem = new(){Uid = testItems[0].Uid, Code = "edited item 1", Description = "Description of edited item 1", ItemType=1, ItemLine=1, ItemGroup=1, SupplierId=1};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testItem);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //Capture the time around when the put request happens to check it later
            DateTime roughPutDate = CETDateTime.Now();
            HttpStatusCode putStatus = client.PutAsync($"{ItemUrl}/{testItem.Uid}", putContent).Result.StatusCode;

            var response = client.GetAsync(ItemUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItems = JsonConvert.DeserializeObject<Item[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putStatus);
            Assert.IsTrue(resultItems.Length == testItems.Length);
            
            //Check the modification date
            Assert.IsTrue(resultItems.First(s=>s.Uid==testItem.Uid).UpdatedAt - roughPutDate <= TimeSpan.FromSeconds(60));

            //Clear the date fields so I can just assert using .equals function
            resultItems.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItem.CreatedAt = new();
            testItem.UpdatedAt = new();
            Assert.IsTrue(resultItems.Any(s=>s.Equals(testItem)), "item was not updated properly");
        }

        [TestMethod]
        public void test_delete_item()
        {
            // Arrange

            // Act
            HttpStatusCode deleteStatus = client.DeleteAsync($"{ItemUrl}/{testItems[0].Uid}").Result.StatusCode;

            var response = client.GetAsync(ItemUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItems = JsonConvert.DeserializeObject<Item[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.IsTrue(resultItems.Length == testItems.Length - 1);
            Assert.IsFalse(resultItems.Any(s=>s.Equals(testItems[0])));
        }
    }
}