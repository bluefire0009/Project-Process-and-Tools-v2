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
            new object[] { new List<Warehouse> { new Warehouse(), new Warehouse() }},
            new object[] { new List<Warehouse> { new Warehouse(), new Warehouse(){IsDeleted = true} }}
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
        Assert.IsTrue(result.Count == warehouses.Where(w=>w.IsDeleted==false).Count());
        for (int warehouseIterator = 0; warehouseIterator < result.Count; warehouseIterator++)
        {
            Assert.IsTrue(result[warehouseIterator].Equals(warehouses[warehouseIterator]));
        }
    }
    public static IEnumerable<object[]> WarehousesTestDataGetRange => new List<object[]>
        {
            new object[] { new List<Warehouse> { new Warehouse()}, 1, 2, true},
            new object[] { new List<Warehouse> { new Warehouse()}, -1, 1, true},
            new object[] { new List<Warehouse> {}, 1, 1, true},
            
            new object[] { new List<Warehouse> { new Warehouse()}, 1, 1, false},
            new object[] { new List<Warehouse> { new Warehouse(), new Warehouse() }, 1, 1, false},
            new object[] { new List<Warehouse> { new Warehouse(), new Warehouse(), new Warehouse() }, 2, 2, false}
        };
    [TestMethod]
    [DynamicData(nameof(WarehousesTestDataGetRange), DynamicDataSourceType.Property)]
    public void TestGetRange(List<Warehouse> warehouses, int offset, int amountToTake, bool nullExpected)
    {
        // Arrange
        foreach (Warehouse warehouse in warehouses)
        {
            db.Warehouses.Add(warehouse);
            db.SaveChanges();
        }
        WarehouseDBStorage storage = new(db);

        // Act
        IEnumerable<Warehouse>? resultOrNull = storage.getWarehousesRange(offset, amountToTake).Result;
        List<Warehouse> result = new();
        if (!(resultOrNull == null)) 
            result = resultOrNull.ToList();

        // Assert
        if (nullExpected == false)
        {
            Assert.IsTrue(result.Count >= offset && result.Count <= amountToTake+offset);
            Assert.IsTrue(result.All(w => w.Id >= offset && w.Id <= amountToTake+offset));
        }
        
        if (nullExpected == true)
        {
            if (offset > 0)
                Assert.IsTrue(warehouses.Count <= offset);
            Assert.IsTrue(offset+amountToTake >= result.Count);
            Assert.IsTrue(resultOrNull == null);
        } 
    }

    public static IEnumerable<object[]> SpecificWarehousesTestData => new List<object[]>
        {
            new object[] { new List<Warehouse> {}, 1, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, 2, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 2, IsDeleted = true}}, 2, false},
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
            new object[] { new Warehouse(){Id = -1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"}, false},
            new object[] { new Warehouse(){Id = 0, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"}, false},
            new object[] { new Warehouse(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"}, true}
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
        if (expectedResult == true)
            Assert.IsTrue(db.Warehouses.Contains(warehouse));
        if (expectedResult == false)
            Assert.IsTrue(!db.Warehouses.Contains(warehouse));        
    }

    [TestMethod]
    public void TestAddSameIdTwice()
    {
        // Arrange
        Warehouse w1 = new() { Id = 1 , Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"};
        Warehouse w2 = new() { Id = 1 , Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"};
        WarehouseDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.addWarehouse(w1).Result;
        bool secondAdd = storage.addWarehouse(w2).Result;

        // Assert
        Assert.IsTrue(firstAdd == true);
        Assert.IsTrue(secondAdd == true);
        // Assert that Id of w2 changed because it was auto assigned by storage
        Assert.IsTrue(w1.Id != w2.Id);
    }

    public static IEnumerable<object[]> RemoveWarehouseTestData => new List<object[]>
        {
            new object[] { new List<Warehouse> {}, 1, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, 0, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, -1, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 1}}, 2, false},
            new object[] { new List<Warehouse> { new Warehouse(){Id = 2, IsDeleted = true}}, 2, false},
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
        if (expectedResult == true)
            Assert.IsTrue(db.Warehouses.Count() == warehouses.Count -1);
        if (expectedResult == false)
            Assert.IsTrue(db.Warehouses.Count() == warehouses.Where(w=>w.IsDeleted==false).Count());
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
            new object[] 
            { 
                new List<Warehouse> {}, 2, 
                new Warehouse(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"},
                false
            },
            new object[] { new List<Warehouse> {}, 1, null,false},
            new object[] 
            { 
                new List<Warehouse> {}, 0, 
                new Warehouse(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"},
                false
            },
            new object[] 
            { 
                new List<Warehouse> {}, -1, 
                new Warehouse(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"},
                false
                },
            new object[] 
            { 
                new List<Warehouse> {new Warehouse(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363" }}, 1, 
                new Warehouse(){Id = 2, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"}, 
                false
            },
            new object[] 
            { 
                new List<Warehouse> {new Warehouse(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363", IsDeleted = true}}, 1, 
                new Warehouse(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"}, 
                false
            },
            new object[] 
            { 
                new List<Warehouse> {new Warehouse(){Id = 1}}, 1, 
                new Warehouse(){Id = 1, Code = "YQZZNL56", Name = "Heemskerk cargo hub", Address = "Karlijndreef 281", Zip = "4002 AS", City = "Heemskerk", Province = "Friesland", Country = "NL", ContactEmail = "blamore@example.net", ContactName = "Fem Keijzer", ContactPhone = "(078) 0013363"}, 
                true
            },
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
        Assert.IsTrue(actualResult == expectedResult);
        if (expectedResult == true)
            Assert.IsTrue(db.Warehouses.Contains(updatedWarehouse));
        if (expectedResult == false)
            Assert.IsTrue(!db.Warehouses.Contains(updatedWarehouse));
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