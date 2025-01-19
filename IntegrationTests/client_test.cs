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
    public class ClientIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private static string ClientUrl = "/api/v2/clients";
        private static string WarehouseUrl = "/api/v2/warehouses";
        private static string InventoryUrl = "/api/v2/inventories";
        private static string ItemUrl = "/api/v2/items";
        private static string LocationUrl = "/api/v2/locations";
        private static string ItemTypeUrl = "/api/v2/itemtypes";
        private static string ItemLineUrl = "/api/v2/itemlines";
        private static string ItemGroupUrl = "/api/v2/item_groups";
        private static string SupplierUrl = "/api/v2/suppliers";
        private static string OrdersUrl = "/api/v2/orders";
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
        private Order[] testOrders = [
            new Order() {Notes = "testorder_1", BillTo = 1, ShipTo = 1},
            new Order() {Notes = "testorder_2", BillTo = 2, ShipTo = 1}
        ];
        private Client[] testClients = [
            new Client() {Name = "Milan", Address = "Jasmijnstraat 53", City = "Papnedrecht", ContactEmail = "milan22veersluis@gamil.com", ContactName = "Milan Versluis", ContactPhone = "0638182257", Country = "Netherlands", Province = "Zuid-Holland", ZipCode = "3353 CG"},
            new Client() {Name = "John", Address = "Jasmijnstraat 53", City = "Papnedrecht", ContactEmail = "milan22veersluis@gamil.com", ContactName = "Milan Versluis", ContactPhone = "0638182257", Country = "Netherlands", Province = "Zuid-Holland", ZipCode = "3353 CG"}
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
            bool warehouseResponse = addTestResourceToDB(client, [testWarehouse], WarehouseUrl);
            bool supplierResponse = addTestResourceToDB(client, testSuppliers, SupplierUrl);
            bool itemGroupResponse = addTestResourceToDB(client, testItemGroups, ItemGroupUrl);
            bool itemLineResponse = addTestResourceToDB(client, testItemLines, ItemLineUrl);
            bool itemTypeResponse = addTestResourceToDB(client, testItemTypes, ItemTypeUrl);
            bool itemResponse = addTestResourceToDB(client, testItems, ItemUrl);
            bool locationsResponse = addTestResourceToDB(client, testLocations, LocationUrl);
            bool inventoryResponse = addTestResourceToDB(client, testInventories, InventoryUrl);
            bool ClientResponse = addTestResourceToDB(client,testClients, ClientUrl);
            bool OrderResponse = addTestResourceToDB(client, testOrders, OrdersUrl);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();  // Optionally delete the database
            }
        }

        [TestMethod]
        public async Task test_get_all()
        {
            // Act
            var response = await client.GetAsync(ClientUrl);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var clients = JsonConvert.DeserializeObject<List<Client>>(responseBody);

            Assert.IsNotNull(clients);
            Assert.AreEqual(testClients.Length, clients.Count);
        }
        [TestMethod]
        public async Task test_get_one()
        {
            // Arrange
            int idToRetrieve = 1;

            // Act
            var response = await this.client.GetAsync($"{ClientUrl}/{idToRetrieve}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var client = JsonConvert.DeserializeObject<Client>(responseBody);

            Assert.IsNotNull(client);
            Assert.AreEqual(idToRetrieve, client.Id);
        }
        [TestMethod]
        public async Task test_post()
        {
            // Arrange
            var newClient = new Client
            {
                Id= 3,
                Name = "New Client",
                Address = "123 New St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Netherlands",
                ContactName = "Test Contact",
                ContactPhone = "1234567890",
                ContactEmail = "test@example.com"
            };
            var postContent = new StringContent(JsonConvert.SerializeObject(newClient), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(ClientUrl, postContent);
            response.EnsureSuccessStatusCode();

            // Assert
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseBody.Contains("Added client"));
        }

        [TestMethod]
        public async Task test_put()
        {
            // Arrange
            int idToUpdate = 1;
            var updatedClient = new Client()
            {
                Id= idToUpdate,
                Name = "Updated Client",
                Address = "123 New St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Netherlands",
                ContactName = "Test Contact",
                ContactPhone = "1234567890",
                ContactEmail = "test@example.com"
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedClient), Encoding.UTF8, "application/json");

            // Act
            var response = await this.client.PutAsync($"{ClientUrl}/{idToUpdate}", jsonContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var getResponse = await this.client.GetAsync($"{ClientUrl}/{idToUpdate}");
            var responseBody = await getResponse.Content.ReadAsStringAsync();
            var client = JsonConvert.DeserializeObject<Client>(responseBody);

            Assert.AreEqual(updatedClient.Name, client.Name);
        }

        [TestMethod]
        public async Task test_delete()
        /*
        {
            // Arrange
            int clientId = testClients[1].Id;

            // Act
            var response = await client.DeleteAsync($"{ClientUrl}/{clientId}");
            response.EnsureSuccessStatusCode();

            // Assert
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseBody.Contains("Deleted client"));
        }
        */
        {
            // Arrange
            int idToDelete = 1;

            // Act
            var response = await client.DeleteAsync($"{ClientUrl}/{idToDelete}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var getResponse = await client.GetAsync($"{ClientUrl}/{idToDelete}");
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [TestMethod]
        public async Task test_get_client_orders()
        {
            // Arrange
            int clientId = 1;

            // Act
            var response = await client.GetAsync($"{ClientUrl}/{clientId}/orders");
            response.EnsureSuccessStatusCode();

            // Assert
            var responseBody = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);
            Assert.IsNotNull(orders);
            Assert.IsTrue(orders.Count > 0);
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