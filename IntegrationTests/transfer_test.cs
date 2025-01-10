using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using CargoHub.HelperFuctions;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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
        private static string InventoryUrl = "/api/v2/inventories";
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
            new(){Id = 1, Reference = "A transfer", TransferFrom = 1, TransferTo = 2, Items = [new() {ItemUid = "P999999", TransferId = 1, Amount = 10}]},
            new(){Id = 2, Reference = "B transfer", TransferFrom = 2, TransferTo = 1, Items = [new() {ItemUid = "P999999", TransferId = 2, Amount = 10}]}
        ];
        private Inventory testInventory = new(){Id = 1, ItemId = "P999999", Description = "", total_available = 100, total_expected = 100, total_on_hand = 100, total_ordered = 0, ItemReference = "", InventoryLocations = {new(){InventoryId = 1, LocationId = 1}}};
        

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
            addTestResourceToDB(client, [testInventory], InventoryUrl);
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
            Transfer testTransfer = new(){Reference = "C transfer", TransferFrom = 2, TransferTo = 2, Items = [new() {ItemUid = "P999999", Amount = 20}]};
            
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

        [TestMethod]
        public void test_put_transfer()
        {
            // Arrange
            Transfer extraTransfer = new(){Reference = "C transfer", TransferFrom = 2, TransferTo = 2, Items = [new() {ItemUid = "P999999", Amount = 20}]};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(extraTransfer);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //Capture the time around when the put request happens to check it later
            DateTime roughPutDate = CETDateTime.Now();
            HttpStatusCode putStatus = client.PutAsync($"{TransferUrl}/{testTransfers[0].Id}", putContent).Result.StatusCode;

            var response = client.GetAsync(TransferUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTransfers = JsonConvert.DeserializeObject<Transfer[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putStatus);
            Assert.IsTrue(resultTransfers.Length == testTransfers.Length);
            
            //Check the modification date
            Assert.IsTrue(resultTransfers.Last().UpdatedAt - roughPutDate <= TimeSpan.FromSeconds(60));

            //Clear the date fields, id's so I can just assert using .equals function
            resultTransfers.ToList().ForEach(t=>{ t.Id = 0; t.Items.ForEach(ti => {ti.TransferId = 0;}); t.CreatedAt = new(); t.UpdatedAt = new();});
            extraTransfer.CreatedAt = new();
            extraTransfer.UpdatedAt = new();
            Assert.IsTrue(resultTransfers.Any(t=>t.Equals(extraTransfer)));
        }

        [TestMethod]
        public void test_commit_transfer()
        {
            // Arrange
            
            // Act
            //Capture the time around when the put request happens to check it later
            DateTime roughCommitDate = CETDateTime.Now();
            HttpStatusCode commitStatus = client.PutAsync($"{TransferUrl}/{testTransfers[0].Id}/commit", null).Result.StatusCode;

            var response = client.GetAsync(TransferUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTransfers = JsonConvert.DeserializeObject<Transfer[]>(content);
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, commitStatus);
            Assert.IsTrue(resultTransfers.First(t => t.Id == testTransfers[0].Id).TransferStatus == "Processed");
            // Check if location has been added inventoryLocations, if yes the transfer has been ACTUALLY processed
            Inventory inventoryTransferedTo = _dbContext.Inventories.Include(i => i.InventoryLocations).First(i => i.ItemId == testTransfers[0].Items[0].ItemUid);
            Assert.IsTrue(inventoryTransferedTo.InventoryLocations.Select(il => il.LocationId).Contains(testTransfers[0].TransferTo));
        }

        [TestMethod]
        public void test_delete_transfer()
        {
            // Arrange

            // Act
            HttpStatusCode deleteStatus = client.DeleteAsync($"{TransferUrl}/{testTransfers[0].Id}").Result.StatusCode;

            var response = client.GetAsync(TransferUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTransfer = JsonConvert.DeserializeObject<Transfer[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.IsTrue(resultTransfer.Length == testTransfers.Length - 1);
            Assert.IsTrue(!resultTransfer.Any(t=>t.Equals(testTransfers[0])));
        }

        [TestMethod]
        public void test_add_transfer_wrong_format()
        {
            // Arrange
            Dictionary<string, object> testTransfer = new(){{"id", 100000003}, {"refference", "TR00001"}, {"tlansfer_from", null}, {"tlansfer_to", 9229}, {"tlansfer_status", "Scheduled"}, {"created_at", "2000-03-11T13:11:14Z"}, {"updated_at", "2000-03-12T16:11:14Z"}};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testTransfer);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(TransferUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(TransferUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTransfers = JsonConvert.DeserializeObject<Transfer[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, postStatus);
            Assert.IsTrue(resultTransfers.Length == testTransfers.Length);

            //Clear the date fields, transfer status's and id so I can just assert using .equals function
            resultTransfers.ToList().ForEach(t=>{t.Id = 0; t.Items.ForEach(ti => {ti.TransferId = 0;}); t.TransferStatus = null; t.CreatedAt = new(); t.UpdatedAt = new();});
            Assert.IsTrue(!resultTransfers.Any(t=>t.Equals(testTransfer)));
        }

        [TestMethod]
        public void test_put_transfer_wrong_format()
        {
            // Arrange
            Dictionary<string, object> transferWrongFormat = new(){{"id", testTransfers[0].Id}, {"refference", "TR00001"}, {"tlansfer_from", null}, {"tlansfer_to", 9229}, {"tlansfer_status", "Scheduled"}, {"created_at", "2000-03-11T13:11:14Z"}, {"updated_at", "2000-03-12T16:11:14Z"}};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(transferWrongFormat);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode putStatus = client.PutAsync($"{TransferUrl}/{testTransfers[0].Id}", putContent).Result.StatusCode;

            var response = client.GetAsync(TransferUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultTransfers = JsonConvert.DeserializeObject<Transfer[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, putStatus);
            Assert.IsTrue(resultTransfers.Any(w=>w.Id != (int)transferWrongFormat["id"]));
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