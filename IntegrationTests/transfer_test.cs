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
    public class TransferIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private static string WarehouseUrl = "/api/v2/warehouses";
        private static string TransferUrl = "/api/v2/transfers";
        private static string LocationUrl = "/api/v2/locations";
        private static string ItemTypeUrl = "/api/v2/itemtypes";
        private static string ItemLineUrl = "/api/v2/itemlines";
        private static string ItemGroupUrl = "/api/v2/item_groups";
        private static string ItemUrl = "/api/v2/items";
        private static string SupplierUrl = "/api/v2/suppliers";
        private ItemType testItemType = new() { Name = "Test Appliances", Description = "test 123" };
        private ItemLine testItemLine = new() { Name = "Test Appliances", Description = "test 123" };
        private ItemGroup testItemGroup = new() { Name = "Test Appliances", Description = "test 123" };
        private Item testItem = new() { Uid = "P999999", Code = "mYt79640E", Description = "Down-sized system-worthy productivity", ShortDescription = "pass", UpcCode = 25411126, ModelNumber = "ZK-417773-PXy", CommodityCode = "z - 761 - L5A", ItemLine = 1, ItemGroup = 1, ItemType = 1, UnitPurchaseQuantity = 3, UnitOrderQuantity = 18, PackOrderQuantity = 13, SupplierId = 1, SupplierCode = "YQZZNL56", SupplierPartNumber = "ZH - 103509 - MLv" };
        private Warehouse testWarehouse = new Warehouse() { Id = 1, Code = "GIOMNL90", Name = "Petten longterm hub", Address = "Owenweg 731", Zip = "4615 RB", City = "Petten", Province = "Noord-Holland", Country = "NL", ContactEmail = "nickteunissen@example.com", ContactName = "Maud Adryaens", ContactPhone = "+31836 752702" };
        private Supplier testSupplier = new() {Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", AddressExtra = "Boven", ZipCode = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactName = "Fem Keijzer", PhoneNumber = "(078) 0013363", Reference = ":)"};
        private Location[] testLocations =
        [
            new(){WareHouseId= 1, Code= "test_code", Name= "test_name"},
            new(){WareHouseId= 1, Code= "test_code", Name= "test_name"}
        ];
        private Transfer[] testTransfers = 
        [
            new(){Id = 1, Reference = "", TransferFrom = 1, TransferTo = 2, Items = [new() {ItemUid = "P999999", TransferId = 1, Amount = 10}]},
            new(){Id = 2, Reference = "", TransferFrom = 2, TransferTo = 1, Items = [new() {ItemUid = "P999999", TransferId = 2, Amount = 10}]}
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
            addTestResourceToDB(client, [testWarehouse], WarehouseUrl);
            addTestResourceToDB(client, [testSupplier], SupplierUrl);
            addTestResourceToDB(client, [testItemGroup], ItemGroupUrl);
            addTestResourceToDB(client, [testItemLine], ItemLineUrl);
            addTestResourceToDB(client, [testItemType], ItemTypeUrl);
            addTestResourceToDB(client, [testItem], ItemUrl);
            addTestResourceToDB(client, testLocations, LocationUrl);
            addTestResourceToDB(client, testTransfers, TransferUrl);
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
        public void test_get_all()
        {
            // Arrange

            // Act
            var response = client.GetAsync(TransferUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTransfers = JsonConvert.DeserializeObject<Transfer[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testTransfers.Length, resultTransfers.Length);
            
            foreach (Transfer transfer in resultTransfers)
            {
                Assert.IsTrue(transfer.TransferStatus == "Scheduled");
            }

            //Clear the date fields and status fiels of resultTransfers so I can just assert using .equals function
            resultTransfers.ToList().ForEach(t=>{t.CreatedAt = new(); t.UpdatedAt = new(); t.TransferStatus = null;});
            testTransfers.ToList().ForEach(t=>{t.CreatedAt = new(); t.UpdatedAt = new();});

            for(int transferIterator = 0; transferIterator<resultTransfers.Length; transferIterator++)
            {
                Assert.AreEqual(testTransfers[transferIterator], resultTransfers[transferIterator]);
            }
        }

        [TestMethod]
        public void test_post_transfer()
        {
            // Arrange
            Transfer testTransfer = new(){Reference = "", TransferFrom = 2, TransferTo = 2, Items = [new() {ItemUid = "P999999", Amount = 20}]};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testTransfer);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(TransferUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(TransferUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTransfers = JsonConvert.DeserializeObject<Transfer[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, postStatus);
            Assert.IsTrue(resultTransfers.Length == testTransfers.Length + 1);
            Assert.IsTrue(resultTransfers.Last().TransferStatus == "Scheduled");

            //Clear the date fields, transfer status's and id so I can just assert using .equals function
            resultTransfers.ToList().ForEach(t=>{t.Id = 0; t.Items.ForEach(ti => {ti.TransferId = 0;}); t.TransferStatus = null; t.CreatedAt = new(); t.UpdatedAt = new();});
            testTransfer.CreatedAt = new();
            testTransfer.UpdatedAt = new();
            Assert.IsTrue(resultTransfers.Any(t=>t.Equals(testTransfer)));
        }

        [TestMethod]
        public void test_get_one()
        {
            // Arrange

            // Act
            var response = client.GetAsync($"{TransferUrl}/{testTransfers[0].Id}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTransfer = JsonConvert.DeserializeObject<Transfer>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            //Clear the date fields, transferStatus so I can just assert using .equals function
            testTransfers.ToList().ForEach(t=>{t.TransferStatus = null; t.CreatedAt = new(); t.UpdatedAt = new();});
            resultTransfer.CreatedAt = new();
            resultTransfer.UpdatedAt = new();
            resultTransfer.TransferStatus = null;
            Assert.IsTrue(testTransfers.Any(t=>t.Equals(resultTransfer)));
        }

        private static void addTestResourceToDB<T>(HttpClient client, T[] resourceArray, string url)
        {
            // Add both transfers to db
            foreach(T resource in resourceArray)
            {
                string jsonData = JsonConvert.SerializeObject(resource);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync($"{url}", postContent).GetAwaiter().GetResult();
            }
        }
    }
}