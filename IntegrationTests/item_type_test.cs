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
        private string ItemUrl = "/api/v2/items";
        private string ItemLineUrl = "/api/v2/itemlines";
        private string ItemTypeUrl = "/api/v2/itemtypes";
        private string ItemGroupUrl = "/api/v2/item_groups";
        private string SupplierUrl = "/api/v2/suppliers";
        private ItemType[] testItemTypes = [
                new ItemType(){Id = 1, Name = "type 1", Description = "Description of itemType 1"},
                new ItemType(){Id = 2, Name = "type 2", Description = "Description of itemType 2"}
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
        public void test_delete_itemType()
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

        [TestMethod]
        public void test_get_items_by_itemType() {
            // Arrange
            // test Item Line creation
            bool itemLineCreation;
            try {
                addTestItemLinesToDB(client);
                itemLineCreation = true;
            } catch {
                itemLineCreation = false;
            }

            Assert.IsTrue(itemLineCreation, "ItemLine creation failed, run itemline intergration tests for more information");

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