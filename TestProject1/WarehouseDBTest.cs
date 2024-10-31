using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class WarehouseDBTest
{
    private DatabaseContext db;
    [TestInitialize]
    public void setUp()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
        db = new DatabaseContext(options);
    }
    
    public static IEnumerable<object[]> WarehousesTestData => new List<object[]>
        {
            new object[] { new List<Warehouse> {}},
            new object[] { new List<Warehouse> { new Warehouse()}},
            new object[] { new List<Warehouse> { new Warehouse(), new Warehouse() }}
        };
    [TestMethod]
    [DynamicData(nameof(WarehousesTestData), DynamicDataSourceType.Property)]
    public void TestGetAll(List<Warehouse> warehouses)
    {
        // Arrange
        foreach(Warehouse warehouse in warehouses)
        {
            db.Warehouses.Add(warehouse);
            db.SaveChanges();
        }

        // Act
        WarehouseDBStorage storage = new(db);
        List<Warehouse> result = storage.getWarehouses().ToList();

        // Assert
        Assert.IsTrue(result.Count == warehouses.Count);
        for(int warehouseIterator = 0; warehouseIterator < result.Count; warehouseIterator++)
        {
            Assert.IsTrue(result[warehouseIterator].Id == warehouses[warehouseIterator].Id);
            Assert.IsTrue(result[warehouseIterator].Code == warehouses[warehouseIterator].Code);
            Assert.IsTrue(result[warehouseIterator].Name == warehouses[warehouseIterator].Name);
            Assert.IsTrue(result[warehouseIterator].Address == warehouses[warehouseIterator].Address);
            Assert.IsTrue(result[warehouseIterator].Zip == warehouses[warehouseIterator].Zip);
            Assert.IsTrue(result[warehouseIterator].City == warehouses[warehouseIterator].City);
            Assert.IsTrue(result[warehouseIterator].Province == warehouses[warehouseIterator].Province);
            Assert.IsTrue(result[warehouseIterator].Country == warehouses[warehouseIterator].Country);
            Assert.IsTrue(result[warehouseIterator].ContactName == warehouses[warehouseIterator].ContactName);
            Assert.IsTrue(result[warehouseIterator].ContactEmail == warehouses[warehouseIterator].ContactEmail);
            Assert.IsTrue(result[warehouseIterator].ContactPhone == warehouses[warehouseIterator].ContactPhone);
            Assert.IsTrue(result[warehouseIterator].CreatedAt == warehouses[warehouseIterator].CreatedAt);
            Assert.IsTrue(result[warehouseIterator].UpdatedAt == warehouses[warehouseIterator].UpdatedAt);
        }
    }
}