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
    public class SupplierIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private string SupplierUrl = "/api/v2/suppliers";
        private string ItemUrl = "/api/v2/items";
        private string ItemLineUrl = "/api/v2/itemlines";
        private string ItemTypeUrl = "/api/v2/itemtypes";
        private string ItemGroupUrl = "/api/v2/item_groups";
        private Supplier[] testSuppliers = [
                new Supplier(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", AddressExtra = "Boven", ZipCode = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactName = "Fem Keijzer", PhoneNumber = "(078) 0013363", Reference = ":)"},
                new Supplier(){Id = 2, Code = "GIOMNL90", Name = "Petten longterm hub", Address = "Owenweg 731", AddressExtra = "Beneden", ZipCode = "4615 RB", City = "Petten", Province = "Noord-Holland", Country = "NL", ContactName = "Maud Adryaens", PhoneNumber = "+31836 752702", Reference = ":)"}
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
            addTestSuppliersToDB(client);
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
            var response = client.GetAsync(SupplierUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultSuppliers = JsonConvert.DeserializeObject<Supplier[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testSuppliers.Length, resultSuppliers.Length);
            
            //Clear the date fields so I can just assert using .equals function
            resultSuppliers.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testSuppliers.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});

            for(int supplierIterator = 0; supplierIterator<resultSuppliers.Length; supplierIterator++)
            {
                Assert.AreEqual(testSuppliers[supplierIterator], resultSuppliers[supplierIterator]);
            }
        }

        [TestMethod]
        public void test_post_supplier()
        {
            // Arrange
            Supplier testSupplier = new(){Id = 3, Code = "GIOMNL90", Name = "Petten longterm hub", Address = "Owenweg 731", AddressExtra = "Beneden", ZipCode = "4615 RB", City = "Petten", Province = "Noord-Holland", Country = "NL", ContactName = "Maud Adryaens", PhoneNumber = "+31836 752702", Reference = ":)"};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testSupplier);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(SupplierUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(SupplierUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultSuppliers = JsonConvert.DeserializeObject<Supplier[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, postStatus);
            Assert.IsTrue(resultSuppliers.Length == testSuppliers.Length + 1);
            
            //Clear the date fields so I can just assert using .equals function
            resultSuppliers.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testSupplier.CreatedAt = new();
            testSupplier.UpdatedAt = new();
            Assert.IsTrue(resultSuppliers.Any(s=>s.Equals(testSupplier)));
        }

        [TestMethod]
        public void test_get_one()
        {
            // Arrange

            // Act
            var response = client.GetAsync($"{SupplierUrl}/{testSuppliers[0].Id}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultSupplier = JsonConvert.DeserializeObject<Supplier>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            //Clear the date fields so I can just assert using .equals function
            testSuppliers.ToList().ForEach(w=>{w.CreatedAt = new(); w.UpdatedAt = new();});
            resultSupplier.CreatedAt = new();
            resultSupplier.UpdatedAt = new();

            Assert.IsTrue(testSuppliers.Any(w=>w.Equals(resultSupplier)));
        }

        [TestMethod]
        public void test_put_warehouse()
        {
            // Arrange
            Supplier testSupplier = new(){Id = testSuppliers[0].Id, Code = "GIOMNL90", Name = "Petten longterm hub", Address = "Owenweg 731", AddressExtra = "Beneden", ZipCode = "4615 RB", City = "Petten", Province = "Noord-Holland", Country = "NL", ContactName = "Maud Adryaens", PhoneNumber = "+31836 752702", Reference = ":)"};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testSupplier);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //Capture the time around when the put request happens to check it later
            DateTime roughPutDate = CETDateTime.Now();
            HttpStatusCode putStatus = client.PutAsync($"{SupplierUrl}/{testSupplier.Id}", putContent).Result.StatusCode;

            var response = client.GetAsync(SupplierUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultSuppliers = JsonConvert.DeserializeObject<Supplier[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putStatus);
            Assert.IsTrue(resultSuppliers.Length == testSuppliers.Length);
            
            //Check the modification date
            Assert.IsTrue(resultSuppliers.First(s=>s.Id==testSupplier.Id).UpdatedAt - roughPutDate <= TimeSpan.FromSeconds(60));

            //Clear the date fields so I can just assert using .equals function
            resultSuppliers.ToList().ForEach(s=>{s.CreatedAt = new(); s.UpdatedAt = new();});
            testSupplier.CreatedAt = new();
            testSupplier.UpdatedAt = new();
            Assert.IsTrue(resultSuppliers.Any(s=>s.Equals(testSupplier)));
        }
                
        [TestMethod]
        public void test_delete_supplier()
        {
            // Arrange

            // Act
            HttpStatusCode deleteStatus = client.DeleteAsync($"{SupplierUrl}/{testSuppliers[0].Id}").Result.StatusCode;

            var response = client.GetAsync(SupplierUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultSuppliers = JsonConvert.DeserializeObject<Supplier[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.IsTrue(resultSuppliers.Length == testSuppliers.Length - 1);
            Assert.IsTrue(!resultSuppliers.Any(s=>s.Equals(testSuppliers[0])));
        }

        [TestMethod]
        public void test_post_supplier_wrong_format()
        {
            // Arrange
            Dictionary<string, object> supplierWrongFormat = new() { {"Ad", 1000003}, {"Wcode", "LIGMAL90"}, {"Sname", "Petten shortterm hub"}, {"adress", "Owenweg 666"}, {"sip", "6420 RB"}, {"citty", "Patten"}, {"brovince", "Zuid-Holland"}, {"countri", "DE"}, {"created_,", "2021-02-22 19:55:39"}, {"updated_,", "2009-08-28 23:15:50"} };
            
            // Act
            string jsonData = JsonConvert.SerializeObject(supplierWrongFormat);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(SupplierUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(SupplierUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultSuppliers = JsonConvert.DeserializeObject<Supplier[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, postStatus);
            Assert.IsTrue(resultSuppliers.Length == testSuppliers.Length);
            Assert.IsTrue(resultSuppliers.Any(s=>s.Id != (int)supplierWrongFormat["Ad"]));
        }

        [TestMethod]
        public void test_put_supplier_wrong_format()
        {
            // Arrange
            Dictionary<string, object> supplierWrongFormat = new() { {"Id", testSuppliers[0].Id}, {"Wcode", "LIGMAL90"}, {"Sname", "Petten shortterm hub"}, {"adress", "Owenweg 666"}, {"sip", "6420 RB"}, {"citty", "Patten"}, {"brovince", "Zuid-Holland"}, {"countri", "DE"}, {"created_,", "2021-02-22 19:55:39"}, {"updated_,", "2009-08-28 23:15:50"} };
            
            // Act
            string jsonData = JsonConvert.SerializeObject(supplierWrongFormat);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode putStatus = client.PutAsync($"{SupplierUrl}/{supplierWrongFormat["Id"]}", putContent).Result.StatusCode;

            var response = client.GetAsync(SupplierUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultSuppliers = JsonConvert.DeserializeObject<Supplier[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, putStatus);
            Assert.IsTrue(resultSuppliers.Any(s=>s.Id != (int)supplierWrongFormat["Id"]));
        }
        
        [TestMethod]
        public void test_get_one_items()
        {
            // Arrange
            ItemLine testItemLine = new() {Id = 1, Description = "Winter", Name = "W1"};
            string jsonDataTestItemLine = JsonConvert.SerializeObject(testItemLine);
            HttpContent postContentLine = new StringContent(jsonDataTestItemLine, Encoding.UTF8, "application/json");
            client.PostAsync($"{ItemLineUrl}", postContentLine).GetAwaiter().GetResult();
            
            ItemGroup testItemGroup = new() {Id = 1, Description = "Clothing", Name = "C1"};
            string jsonDataTestItemGroup = JsonConvert.SerializeObject(testItemGroup);
            HttpContent postContentGroup = new StringContent(jsonDataTestItemGroup, Encoding.UTF8, "application/json");
            client.PostAsync($"{ItemGroupUrl}", postContentGroup).GetAwaiter().GetResult();
            
            ItemType testItemType = new() {Id = 1, Description = "Socks", Name = "S1"};
            string jsonDataTestItemType = JsonConvert.SerializeObject(testItemType);
            HttpContent postContentType = new StringContent(jsonDataTestItemType, Encoding.UTF8, "application/json");
            client.PostAsync($"{ItemTypeUrl}", postContentType).GetAwaiter().GetResult();
            
            Item testItem = new() {Uid = "P00001", Code = "1234", CommodityCode = "5678", Description = "Nice socks", ShortDescription=":3", ModelNumber = "9012", PackOrderQuantity = 1, SupplierCode = 1, SupplierId = testSuppliers[0].Id, SupplierPartNumber = "SC01", ItemType = testItemType.Id, ItemGroup = testItemGroup.Id, ItemLine = testItemLine.Id, };
            string jsonDataTestItem = JsonConvert.SerializeObject(testItem);
            HttpContent postContentItem = new StringContent(jsonDataTestItem, Encoding.UTF8, "application/json");
            client.PostAsync($"{ItemUrl}", postContentItem).GetAwaiter().GetResult();

            // Act
            var response = client.GetAsync($"{SupplierUrl}/{testSuppliers[0].Id}/items").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Item[] resultItems = JsonConvert.DeserializeObject<Item[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            //Clear the date fields so I can just assert using .equals function
            resultItems.ToList().ForEach(l=>{l.CreatedAt = new(); l.UpdatedAt = new();});
            testItem.CreatedAt = new();
            testItem.UpdatedAt = new();

            Assert.IsTrue(resultItems.Any(l=>l.Equals(testItem)));
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
    }
}