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
    public class WarehouseIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private string WarehouseUrl = "/api/v2/warehouses";
        private string LocationUrl = "/api/v2/locations";
        private Warehouse[] testWarehouses = [
                new Warehouse(){Id = 1000001, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363", CreatedAt = new(1983,04,13,4,59,55), UpdatedAt = new(2007,02,08,20,11,00)},
                new Warehouse(){Id = 1000002, Code = "GIOMNL90", Name = "Petten longterm hub", Address = "Owenweg 731", Zip = "4615 RB", City = "Petten", Province = "Noord-Holland", Country = "NL", ContactEmail = "nickteunissen@example.com", ContactName = "Maud Adryaens", ContactPhone = "+31836 752702", CreatedAt = new(2008,02,22,19,55,39), UpdatedAt = new(2009,08,28,23,15,50)}
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
            addTestWarehousesToDB(client);
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
            var response = client.GetAsync(WarehouseUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultWarehouses = JsonConvert.DeserializeObject<Warehouse[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testWarehouses.Length, resultWarehouses.Length);
            
            //Clear the date fields so I can just assert using .equals function
            resultWarehouses.ToList().ForEach(w=>{w.CreatedAt = new(); w.UpdatedAt = new();});
            testWarehouses.ToList().ForEach(w=>{w.CreatedAt = new(); w.UpdatedAt = new();});
            for(int warehouseIterator = 0; warehouseIterator<resultWarehouses.Length; warehouseIterator++)
            {
                Assert.AreEqual(testWarehouses[warehouseIterator], resultWarehouses[warehouseIterator]);
            }
        }

        [TestMethod]
        public void test_post_warehouse()
        {
            // Arrange
            Warehouse testWarehouse = new(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363", CreatedAt = new(1983,04,13,4,59,55), UpdatedAt = new(2007,02,08,20,11,00)};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testWarehouse);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(WarehouseUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(WarehouseUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultWarehouses = JsonConvert.DeserializeObject<Warehouse[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, postStatus);
            Assert.IsTrue(resultWarehouses.Length == testWarehouses.Length + 1);
            
            //Clear the date fields so I can just assert using .equals function
            resultWarehouses.ToList().ForEach(w=>{w.CreatedAt = new(); w.UpdatedAt = new();});
            testWarehouse.CreatedAt = new();
            testWarehouse.UpdatedAt = new();
            Assert.IsTrue(resultWarehouses.Any(w=>w.Equals(testWarehouse)));
        }

        [TestMethod]
        public void test_get_one()
        {
            // Arrange

            // Act
            var response = client.GetAsync($"{WarehouseUrl}/{testWarehouses[0].Id}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultWarehouse = JsonConvert.DeserializeObject<Warehouse>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            //Clear the date fields so I can just assert using .equals function
            testWarehouses.ToList().ForEach(w=>{w.CreatedAt = new(); w.UpdatedAt = new();});
            resultWarehouse.CreatedAt = new();
            resultWarehouse.UpdatedAt = new();

            Assert.IsTrue(testWarehouses.Any(w=>w.Equals(resultWarehouse)));
        }

        [TestMethod]
        public void test_put_warehouse()
        {
            // Arrange
            Warehouse testWarehouse = new(){Id = testWarehouses[0].Id, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363", CreatedAt = new(1983,04,13,4,59,55), UpdatedAt = new(2007,02,08,20,11,00)};
            
            // Act
            string jsonData = JsonConvert.SerializeObject(testWarehouse);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //Capture the time around when the put request happens to check it later
            DateTime roughPutDate = CETDateTime.Now();
            HttpStatusCode putStatus = client.PutAsync($"{WarehouseUrl}/{testWarehouse.Id}", putContent).Result.StatusCode;

            var response = client.GetAsync(WarehouseUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultWarehouses = JsonConvert.DeserializeObject<Warehouse[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putStatus);
            Assert.IsTrue(resultWarehouses.Length == testWarehouses.Length);
            
            //Check the modification date
            Assert.IsTrue(resultWarehouses.First(w=>w.Id==testWarehouse.Id).UpdatedAt - roughPutDate <= TimeSpan.FromSeconds(60));

            //Clear the date fields so I can just assert using .equals function
            resultWarehouses.ToList().ForEach(w=>{w.CreatedAt = new(); w.UpdatedAt = new();});
            testWarehouse.CreatedAt = new();
            testWarehouse.UpdatedAt = new();
            Assert.IsTrue(resultWarehouses.Any(w=>w.Equals(testWarehouse)));
        }

        [TestMethod]
        public void test_delete_warehouse()
        {
            // Arrange

            // Act
            HttpStatusCode deleteStatus = client.DeleteAsync($"{WarehouseUrl}/{testWarehouses[0].Id}").Result.StatusCode;

            var response = client.GetAsync(WarehouseUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultWarehouses = JsonConvert.DeserializeObject<Warehouse[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.IsTrue(resultWarehouses.Length == testWarehouses.Length - 1);
            Assert.IsTrue(!resultWarehouses.Any(w=>w.Equals(testWarehouses[0])));
        }

        [TestMethod]
        public void test_get_one_locations()
        {
            // Arrange
            Location testLocation = new() {Id = 1, Name = "AAA", Code = "123", WareHouseId = testWarehouses[0].Id};
            string jsonData = JsonConvert.SerializeObject(testLocation);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            client.PostAsync($"{LocationUrl}", postContent).GetAwaiter().GetResult();

            // Act
            var response = client.GetAsync($"{WarehouseUrl}/{testWarehouses[0].Id}/locations").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Location[] resultLocations = JsonConvert.DeserializeObject<Location[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            //Clear the date fields so I can just assert using .equals function
            resultLocations.ToList().ForEach(l=>{l.CreatedAt = new(); l.UpdatedAt = new();});
            testLocation.CreatedAt = new();
            testLocation.UpdatedAt = new();

            Assert.IsTrue(resultLocations.Any(l=>l.Equals(testLocation)));
        }

        [TestMethod]
        public void test_get_get_all_range()
        {
            // Arrange

            // Act
            //                                    localhost:3000/api/v2/warehouses/Range?firstIdToTake=1&amountToTake=2
            var response = client.GetAsync($"{WarehouseUrl}/range?firstIdToTake=1&amountToTake=1").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Warehouse[] resultWarehouses = JsonConvert.DeserializeObject<Warehouse[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            //Clear the date fields so I can just assert using .equals function
            resultWarehouses.ToList().ForEach(w=>{w.CreatedAt = new(); w.UpdatedAt = new();});
            testWarehouses.ToList().ForEach(w=>{w.CreatedAt = new(); w.UpdatedAt = new();});

            Assert.IsTrue(resultWarehouses.Any(w=>w.Equals(testWarehouses[0])));
        }

        [TestMethod]
        public void test_post_warehouse_wrong_format()
        {
            // Arrange
            Dictionary<string, object> warehouseWrongFormat = new() { {"Id", 1000003}, {"Wcode", "LIGMAL90"}, {"Sname", "Petten shortterm hub"}, {"adress", "Owenweg 666"}, {"sip", "6420 RB"}, {"citty", "Patten"}, {"brovince", "Zuid-Holland"}, {"countri", "DE"}, {"created_,", "2021-02-22 19:55:39"}, {"updated_,", "2009-08-28 23:15:50"} };
            
            // Act
            string jsonData = JsonConvert.SerializeObject(warehouseWrongFormat);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode postStatus = client.PostAsync(WarehouseUrl, postContent).Result.StatusCode;

            var response = client.GetAsync(WarehouseUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultWarehouses = JsonConvert.DeserializeObject<Warehouse[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, postStatus);
            Assert.IsTrue(resultWarehouses.Length == testWarehouses.Length);
            Assert.IsTrue(resultWarehouses.Any(w=>w.Id != (int)warehouseWrongFormat["Id"]));
        }

        [TestMethod]
        public void test_put_warehouse_wrong_format()
        {
            // Arrange
            Dictionary<string, object> warehouseWrongFormat = new() { {"Id", testWarehouses[0].Id}, {"Wcode", "LIGMAL90"}, {"Sname", "Petten shortterm hub"}, {"adress", "Owenweg 666"}, {"sip", "6420 RB"}, {"citty", "Patten"}, {"brovince", "Zuid-Holland"}, {"countri", "DE"}, {"created_,", "2021-02-22 19:55:39"}, {"updated_,", "2009-08-28 23:15:50"} };
            
            // Act
            string jsonData = JsonConvert.SerializeObject(warehouseWrongFormat);
            HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpStatusCode putStatus = client.PutAsync($"{WarehouseUrl}/{warehouseWrongFormat["Id"]}", putContent).Result.StatusCode;

            var response = client.GetAsync(WarehouseUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var resultWarehouses = JsonConvert.DeserializeObject<Warehouse[]>(content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, putStatus);
            Assert.IsTrue(resultWarehouses.Any(w=>w.Id != (int)warehouseWrongFormat["Id"]));
        }

        private void addTestWarehousesToDB(HttpClient client)
        {
            // Add both warehouses to db
            foreach(Warehouse warehouse in testWarehouses)
            {
                string jsonData = JsonConvert.SerializeObject(warehouse);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync($"{WarehouseUrl}", postContent).GetAwaiter().GetResult();
            }
        }
    }
}
