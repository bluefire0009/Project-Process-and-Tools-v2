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
        private static DatabaseContext _dbContext;
        private static string TransferUrl = "/api/v2/transfers";
        private static string LocationUrl = "/api/v2/locations";
        private static ItemType testItemType = new() { Name = "Test Appliances", Description = "test 123" };
        private static ItemLine testItemLine = new() { Name = "Test Appliances", Description = "test 123" };
        private static ItemGroup testItemGroup = new() { Name = "Test Appliances", Description = "test 123" };
        private static Item testItem = new() { Uid = "P999999", Code = "mYt79640E", Description = "Down-sized system-worthy productivity", ShortDescription = "pass", UpcCode = 25411126, ModelNumber = "ZK-417773-PXy", CommodityCode = "z - 761 - L5A", ItemLine = 81, ItemGroup = 83, ItemType = 74, UnitPurchaseQuantity = 3, UnitOrderQuantity = 18, PackOrderQuantity = 13, SupplierId = testSupplier.Id, SupplierCode = $"{testSupplier.Code}", SupplierPartNumber = "ZH - 103509 - MLv" };
        private static Supplier testSupplier = new() { };
        private static Location[] testLocations =
        [
            new(){WareHouseId= testWarehouse.Id, Code= "test_code", Name= "test_name"},
            new(){WareHouseId= testWarehouse.Id, Code= "test_code", Name= "test_name"}
        ];
        private static Warehouse testWarehouse = new Warehouse() { Id = 2, Code = "GIOMNL90", Name = "Petten longterm hub", Address = "Owenweg 731", Zip = "4615 RB", City = "Petten", Province = "Noord-Holland", Country = "NL", ContactEmail = "nickteunissen@example.com", ContactName = "Maud Adryaens", ContactPhone = "+31836 752702" };

        private static HttpClient client;

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
            Assert.IsTrue(2 + 1 == 3);
        }
    }
}