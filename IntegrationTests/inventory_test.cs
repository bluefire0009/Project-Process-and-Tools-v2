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
    public class InventoriesIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private static string WarehouseUrl = "/api/v2/warehouses";
        private static string InventoryUrl = "/api/v2/inventories";
        private static string ItemUrl = "/api/v2/items";
        private static string LocationUrl = "/api/v2/locations";
        private static string ItemTypeUrl = "/api/v2/itemtypes";
        private static string ItemLineUrl = "/api/v2/itemlines";
        private static string ItemGroupUrl = "/api/v2/item_groups";
        private static string SupplierUrl = "/api/v2/suppliers";
        private ItemType[] testItemTypes = [
                new ItemType(){Id = 1, Name = "type 1", Description = "Description of itemType 1"}
            ];
        
        private Inventory[] testInventories = [
                new Inventory(){ItemId = "P00001", Description = "Description of Inventory 1", ItemReference = "Refernce of Inventory 1", total_on_hand = 10, total_allocated = 5, total_available = 30},
                new Inventory(){ItemId = "P00002", Description = "Description of Inventory 2", ItemReference = "Refernce of Inventory 2", total_on_hand = 10, total_allocated = 5, total_available = 30}
            ];
        
        private Item[] testItems = [
                new Item(){Uid = "P00001", ItemType=1, ItemLine=1, ItemGroup=1, SupplierId=1},
                new Item(){Uid = "P00002", ItemType=1, ItemLine=1, ItemGroup=1, SupplierId=1},
                new Item(){Uid = "P00003", ItemType=1, ItemLine=1, ItemGroup=1, SupplierId=1}
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
                new Supplier(){Id = 2, Code = "GIOMNL90", Name = "Petten longterm hub", Address = "Owenweg 731", AddressExtra = "Beneden", ZipCode = "4615 RB", City = "Petten", Province = "Noord-Holland", Country = "NL", ContactName = "Maud Adryaens", PhoneNumber = "+31836 752702", Reference = ":)"}
            ];
        private Location[] testLocations =
        [
            new(){WareHouseId= 1, Code= "test_code", Name= "test_name"},
            new(){WareHouseId= 1, Code= "test_code", Name= "test_name"}
        ];
        private Warehouse testWarehouse = new Warehouse() { Id = 1, Code = "GIOMNL90", Name = "Petten longterm hub", Address = "Owenweg 731", Zip = "4615 RB", City = "Petten", Province = "Noord-Holland", Country = "NL", ContactEmail = "nickteunissen@example.com", ContactName = "Maud Adryaens", ContactPhone = "+31836 752702" };

        
        private HttpClient client;

        [TestInitialize]
        public void Setup()
        /*
        {
            // Get the service provider and create a new scope for each test
            var scope = Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            // Reset the database to ensure it is clean for each test
            _dbContext.Database.EnsureDeleted();  // Delete any existing database
            _dbContext.Database.EnsureCreated();  // Create a new fresh database
            client = CreateClient();
            addTestResourceToDB(client);
            addTestInventoriesToDB(client);
        }
        */
        {
            // Get the service provider and create a new scope for each test
            var scope = Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            // Reset the database to ensure it is clean for each test
            _dbContext.Database.EnsureDeleted();  // Delete any existing database
            _dbContext.Database.EnsureCreated();  // Create a new fresh database
            client = CreateClient();
            bool warehouseResponse = addTestResourceToDB(client, [testWarehouse], WarehouseUrl);
            bool supplierResponse = addTestResourceToDB(client, testSuppliers, SupplierUrl);
            bool itemGroupResponse = addTestResourceToDB(client, testItemGroups, ItemGroupUrl);
            bool itemLineResponse = addTestResourceToDB(client, testItemLines, ItemLineUrl);
            bool itemTypeResponse = addTestResourceToDB(client, testItemTypes, ItemTypeUrl);
            bool itemResponse = addTestResourceToDB(client, testItems, ItemUrl);
            bool locationsResponse = addTestResourceToDB(client, testLocations, LocationUrl);
            bool inventoryResponse = addTestResourceToDB(client, testInventories, InventoryUrl);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();  // Optionally delete the database
            }
        }

        private void addTestInventoriesToDB(HttpClient client)
        {
            // Add both Suppliers to db
            foreach(Inventory inventory in testInventories)
            {
                string jsonData = JsonConvert.SerializeObject(inventory);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync($"{InventoryUrl}", postContent).GetAwaiter().GetResult();
            }
        }
        [TestMethod]
        public async Task test_get_all()
        {
            // Act
            var response = await client.GetAsync(InventoryUrl);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var inventories = JsonConvert.DeserializeObject<List<Inventory>>(responseBody);

            Assert.IsNotNull(inventories);
            Assert.AreEqual(testInventories.Length, inventories.Count);
        }

        [TestMethod]
        public async Task test_get_one()
        {
            // Arrange
            int idToRetrieve = 1;

            // Act
            var response = await client.GetAsync($"{InventoryUrl}/{idToRetrieve}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var inventory = JsonConvert.DeserializeObject<Inventory>(responseBody);

            Assert.IsNotNull(inventory);
            Assert.AreEqual(idToRetrieve, inventory.Id);
        }

        [TestMethod]
        public async Task test_post()
        {
            // Arrange
            var newInventory = new Inventory()
            {
                Id = 3,
                ItemId = "P00003",
                Description = "Description of Inventory 3",
                ItemReference = "Reference of Inventory 3",
                total_on_hand = 20,
                total_allocated = 5,
                total_available = 15
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newInventory), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(InventoryUrl, jsonContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var getResponse = await client.GetAsync($"{InventoryUrl}/{newInventory.Id}");
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var responseBody = await getResponse.Content.ReadAsStringAsync();
            var inventory = JsonConvert.DeserializeObject<Inventory>(responseBody);
            Assert.AreEqual(newInventory.Description, inventory.Description);
        }

        [TestMethod]
        public async Task test_put()
        {
            // Arrange
            int idToUpdate = 1;
            var updatedInventory = new Inventory()
            {
                Id = idToUpdate,
                ItemId = "P00001",
                Description = "Updated Inventory Description",
                ItemReference = "Updated Inventory Reference",
                total_on_hand = 25,
                total_allocated = 10,
                total_available = 15
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedInventory), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"{InventoryUrl}/{idToUpdate}", jsonContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var getResponse = await client.GetAsync($"{InventoryUrl}/{idToUpdate}");
            var responseBody = await getResponse.Content.ReadAsStringAsync();
            var inventory = JsonConvert.DeserializeObject<Inventory>(responseBody);

            Assert.AreEqual(updatedInventory.Description, inventory.Description);
        }

        [TestMethod]
        public async Task test_delete_inventory()
        {
            // Arrange
            int idToDelete = 1;

            // Act
            var response = await client.DeleteAsync($"{InventoryUrl}/{idToDelete}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var getResponse = await client.GetAsync($"{InventoryUrl}/{idToDelete}");
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        private static bool addTestResourceToDB<T>(HttpClient client, T[] resourceArray, string url)
        {
            // if (resourceArray is Transfer[]) 
            //     Console.WriteLine("");
            List<bool> responses = [];
            // Add both transfers to db
            foreach (T resource in resourceArray)
            {
                string jsonData = JsonConvert.SerializeObject(resource);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpStatusCode responseStatus = client.PostAsync($"{url}", postContent).GetAwaiter().GetResult().StatusCode;
                if (responseStatus == HttpStatusCode.OK || responseStatus == HttpStatusCode.Created) responses.Add(true);
                else responses.Add(false);
            }
            if (responses.All(r => r == true)) return true;
            return false;
        }

    }
}