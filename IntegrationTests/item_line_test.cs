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

        [TestMethod]
        public void test_get_items_by_itemLine() {
            // Arrange
            // test Item Type creation
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

            // test item creation
            bool itemCreation;
            try {
                addTestItemsToDB(client);
                itemCreation = true;
            } catch {
                itemCreation = false;
            }

            Assert.IsTrue(itemCreation, "Item creation failed, run item intergration tests for more information");

            // Act
            var response = client.GetAsync(ItemTypeUrl + "/" + testItemTypes[0].Id + "/items").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultItems = JsonConvert.DeserializeObject<Item[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testItems.Length, resultItems.Length);
            
            //Clear the date fields so I can just assert using .equals function
            resultItems.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testItems.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});

            for(int itemTypeIterator = 0; itemTypeIterator<resultItems.Length; itemTypeIterator++)
            {
                Assert.IsTrue(testItems[itemTypeIterator].Equals(resultItems[itemTypeIterator]));
            }
        }
    }
}