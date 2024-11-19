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
        foreach (Warehouse warehouse in warehouses)
        {
            db.Warehouses.Add(warehouse);
            db.SaveChanges();
        }
        WarehouseDBStorage storage = new(db);

        // Act
        List<Warehouse> result = storage.getWarehouses().Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == warehouses.Count);
        for (int warehouseIterator = 0; warehouseIterator < result.Count; warehouseIterator++)
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

    public static IEnumerable<object[]> SpecificWarehousesTestData => new List<object[]>
        {
            new object[] { new List<Warehouse> {}, 1, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, 2, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, 1, true},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}, new Warehouse(){Id = 2}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(SpecificWarehousesTestData), DynamicDataSourceType.Property)]
    public void TestGetSpecific(List<Warehouse> warehouses, int soughtId, bool expectedResult)
    {
        // Arrange
        foreach (Warehouse warehouse in warehouses)
        {
            db.Warehouses.Add(warehouse);
            db.SaveChanges();
        }
        WarehouseDBStorage storage = new(db);

        // Act
        Warehouse? foundWarehouse = storage.getWarehouse(soughtId).Result;

        // Assert
        bool actualResult = foundWarehouse != null;
        Assert.IsTrue(actualResult == expectedResult);
    }
    public static IEnumerable<object[]> AddWarehouseTestData => new List<object[]>
        {
            new object[] { null, false},
            new object[] { new Warehouse(){Id = -1}, false},
            new object[] { new Warehouse(){Id = 0}, false},
            new object[] { new Warehouse(){Id = 1}, true}
        };
    [TestMethod]
    [DynamicData(nameof(AddWarehouseTestData), DynamicDataSourceType.Property)]
    public void TestAdd(Warehouse warehouse, bool expectedResult)
    {
        // Arrange
        WarehouseDBStorage storage = new(db);

        // Act
        bool actualResult = storage.addWarehouse(warehouse).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    [TestMethod]
    public void TestAddSameIdTwice()
    {
        // Arrange
        Warehouse w1 = new() { Id = 1 };
        Warehouse w2 = new() { Id = 1 };
        WarehouseDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.addWarehouse(w1).Result;
        bool secondAdd = storage.addWarehouse(w2).Result;

        // Assert
        Assert.IsTrue(firstAdd == true);
        Assert.IsTrue(secondAdd == false);
    }

    public static IEnumerable<object[]> RemoveWarehouseTestData => new List<object[]>
        {
            new object[] { new List<Warehouse> {}, 1, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, 0, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, -1, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, 2, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, 1, true},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}, new Warehouse(){Id = 2}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(RemoveWarehouseTestData), DynamicDataSourceType.Property)]
    public void TestRemove(List<Warehouse> warehouses, int idToRemove, bool expectedResult)
    {
        // Arrange
        foreach (Warehouse warehouse in warehouses)
        {
            db.Warehouses.Add(warehouse);
            db.SaveChanges();
        }
        WarehouseDBStorage storage = new(db);

        // Act
        bool actualResult = storage.deleteWarehouse(idToRemove).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    [TestMethod]
    public void TestRemoveSameTwice()
    {
        // Arrange
        Warehouse w1 = new() { Id = 1 };
        db.Warehouses.Add(w1);
        db.SaveChanges();
        WarehouseDBStorage storage = new(db);


        // Act
        bool firstRemove = storage.deleteWarehouse(w1.Id).Result;
        bool secondRemove = storage.deleteWarehouse(w1.Id).Result;

        // Assert
        Assert.IsTrue(firstRemove == true);
        Assert.IsTrue(secondRemove == false);
    }

    public static IEnumerable<object[]> UpdateWarehouseTestData => new List<object[]>
        {
            new object[] { new List<Warehouse> {}, 2, new Warehouse(){Id = 1},false},
            new object[] { new List<Warehouse> {}, 1, null,false},
            new object[] { new List<Warehouse> {}, 0, new Warehouse(){Id = 1},false},
            new object[] { new List<Warehouse> {}, -1, new Warehouse(){Id = 1},false},
            new object[] { new List<Warehouse> {new Warehouse(){Id = 1}}, 1, new Warehouse(){Id = 2}, false},
            new object[] { new List<Warehouse> {new Warehouse(){Id = 1}}, 1, new Warehouse(){Id = 1, Code = "ABC"}, true},
        };
    [TestMethod]
    [DynamicData(nameof(UpdateWarehouseTestData), DynamicDataSourceType.Property)]
    public void TestUpdate(List<Warehouse> warehouses, int idToUpdate, Warehouse updatedWarehouse, bool expectedResult)
    {
        // Arrange
        foreach (Warehouse warehouse in warehouses)
        {
            db.Warehouses.Add(warehouse);
            db.SaveChanges();
        }
        WarehouseDBStorage storage = new(db);

        // Act
        bool actualResult = storage.updateWarehouse(idToUpdate, updatedWarehouse).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> GetWarehouseLocationsTestData => new List<object[]>
        {
            new object[] { new List<Warehouse> {new(){Id = 1}}, new List<Location> {new(){Id = 1, WareHouseId = 1}, new(){Id = 2, WareHouseId = 1}, new(){Id = 3, WareHouseId = 1}}, 1},
            new object[] { new List<Warehouse> {new(){Id = 1}}, null, 0},
            new object[] { new List<Warehouse> {new(){Id = 1}}, null, -1},
            new object[] { new List<Warehouse> {new(){Id = 2}}, new List<Location> {new(){Id = 1, WareHouseId = 1}, new(){Id = 2, WareHouseId = 1}, new(){Id = 3, WareHouseId = 1}}, 2},
        };
    [TestMethod]
    [DynamicData(nameof(GetWarehouseLocationsTestData), DynamicDataSourceType.Property)]
    public void TestGetLocations(List<Warehouse> warehouses, List<Location> locations, int soughtId)
    {
        // Arrange
        foreach (Warehouse warehouse in warehouses)
        {
            db.Warehouses.Add(warehouse);
            db.SaveChanges();
        }
        if (locations != null)
        {
            foreach (Location location in locations)
            {
                db.Locations.Add(location);
                db.SaveChanges();
            }
        }
        WarehouseDBStorage storage = new(db);

        // Act
        IEnumerable<Location>? result = storage.getWarehouseLocations(soughtId);
        List<Location> resultList;
        // these two statements are here because getWarehouseLocations COULD TECHNICALLY return a null, not very likely but still
        if (result == null) resultList = new();
        else resultList = result.ToList();


        // Assert
        for (int locationIterator = 0; locationIterator < resultList.Count; locationIterator++)
        {
            Assert.IsTrue(resultList[locationIterator].WareHouseId == locations[locationIterator].WareHouseId);
            Assert.IsTrue(resultList[locationIterator].Id == locations[locationIterator].Id);
            Assert.IsTrue(resultList[locationIterator].Code == locations[locationIterator].Code);
            Assert.IsTrue(resultList[locationIterator].Name == locations[locationIterator].Name);
            Assert.IsTrue(resultList[locationIterator].CreatedAt == locations[locationIterator].CreatedAt);
            Assert.IsTrue(resultList[locationIterator].UpdatedAt == locations[locationIterator].UpdatedAt);
        }
        if (resultList.Count == 0 && result != null)
            Assert.IsTrue(locations != null);
        if (result == null)
            Assert.IsTrue(locations == null);
    }
}