using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class InventoriesDBTest
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

    public static IEnumerable<object[]> InventoriesTestData => new List<object[]>
    {
        new object[] { new List<Inventory> {} },
        new object[] { new List<Inventory> { new Inventory() } },
        new object[] { new List<Inventory> { new Inventory(), new Inventory() } }
    };
    [TestMethod]
    public void TestGetInventoriesInPagination()
    {
        // Arrange
        var inventories = new List<Inventory>
        {
            new Inventory { Id = 1, ItemId = "Inventory 1" },
            new Inventory { Id = 2, ItemId = "Inventory 2" },
            new Inventory { Id = 3, ItemId = "Inventory 3" },
            new Inventory { Id = 4, ItemId = "Inventory 4" }
        };

        db.Inventories.AddRange(inventories);
        db.SaveChangesAsync();

        int offset = 1; // Start from the second inventory
        int limit = 2;  // Retrieve two inventories

        InventoriesDBStorage storage = new(db);

        // Act
        var result = storage.GetInventoriesInPagination(offset, limit).Result.ToList();

        // Assert
        Assert.AreEqual(limit, result.Count, "The number of retrieved inventories should match the limit.");
        Assert.AreEqual(2, result[0].Id, "The first inventory in the result should match the offset.");
        Assert.AreEqual(3, result[1].Id, "The second inventory in the result should be the next one.");
    }

    [TestMethod]
    [DynamicData(nameof(InventoriesTestData), DynamicDataSourceType.Property)]
    public void TestGetAll(List<Inventory> inventories)
    {
        // Arrange
        foreach (Inventory inventory in inventories)
        {
            db.Inventories.Add(inventory);
            db.SaveChanges();
        }
        InventoriesDBStorage storage = new(db);

        // Act
        List<Inventory> result = storage.getInventories().Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == inventories.Count);
    }

    public static IEnumerable<object[]> SpecificInventoriesTestData => new List<object[]>
    {
        new object[] { new List<Inventory> {}, 1, false },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 } }, 2, false },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 } }, 1, true },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 }, new Inventory() { Id = 2 } }, 2, true }
    };

    [TestMethod]
    [DynamicData(nameof(SpecificInventoriesTestData), DynamicDataSourceType.Property)]
    public void TestGetSpecific(List<Inventory> inventories, int soughtId, bool expectedResult)
    {
        // Arrange
        foreach (Inventory inventory in inventories)
        {
            db.Inventories.Add(inventory);
            db.SaveChanges();
        }
        InventoriesDBStorage storage = new(db);

        // Act
        Inventory? foundInventory = storage.getInventory(soughtId).Result;

        // Assert
        bool actualResult = foundInventory != null;
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> AddInventoryTestData => new List<object[]>
    {
        new object[] { null, false },
        new object[] { new Inventory() { Id = -1 }, false },
        new object[] { new Inventory() { Id = 0 }, false },
        new object[] { new Inventory() { Id = 1 }, true }
    };

    [TestMethod]
    [DynamicData(nameof(AddInventoryTestData), DynamicDataSourceType.Property)]
    public void TestAdd(Inventory inventory, bool expectedResult)
    {
        // Arrange
        InventoriesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.addInventory(inventory).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    [TestMethod]
    public void TestAddSameIdTwice()
    {
        // Arrange
        Inventory i1 = new() { Id = 1 };
        Inventory i2 = new() { Id = 1 };
        InventoriesDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.addInventory(i1).Result;
        bool secondAdd = storage.addInventory(i2).Result;

        // Assert
        Assert.IsTrue(firstAdd == true);
        Assert.IsTrue(secondAdd == false);
    }

    public static IEnumerable<object[]> RemoveInventoryTestData => new List<object[]>
    {
        new object[] { new List<Inventory> {}, 1, false },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 } }, 0, false },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 } }, -1, false },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 } }, 2, false },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 } }, 1, true },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 }, new Inventory() { Id = 2 } }, 2, true }
    };

    [TestMethod]
    [DynamicData(nameof(RemoveInventoryTestData), DynamicDataSourceType.Property)]
    public void TestRemove(List<Inventory> inventories, int idToRemove, bool expectedResult)
    {
        // Arrange
        foreach (Inventory inventory in inventories)
        {
            db.Inventories.Add(inventory);
            db.SaveChanges();
        }
        InventoriesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.deleteInventory(idToRemove).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    [TestMethod]
    public void TestRemoveSameTwice()
    {
        // Arrange
        Inventory i1 = new() { Id = 1 };
        db.Inventories.Add(i1);
        db.SaveChanges();
        InventoriesDBStorage storage = new(db);

        // Act
        bool firstRemove = storage.deleteInventory(i1.Id).Result;
        bool secondRemove = storage.deleteInventory(i1.Id).Result;

        // Assert
        Assert.IsTrue(firstRemove == true);
        Assert.IsTrue(secondRemove == false);
    }

    public static IEnumerable<object[]> UpdateInventoryTestData => new List<object[]>
    {
        new object[] { new List<Inventory> {}, 2, new Inventory() { Id = 1 }, false },
        new object[] { new List<Inventory> {}, 1, null, false },
        new object[] { new List<Inventory> {}, 0, new Inventory() { Id = 1 }, false },
        new object[] { new List<Inventory> {}, -1, new Inventory() { Id = 1 }, false },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 } }, 1, new Inventory() { Id = 1 }, true },
        new object[] { new List<Inventory> { new Inventory() { Id = 1 } }, 1, new Inventory() { Id = 2 }, true }
    };

    [TestMethod]
    [DynamicData(nameof(UpdateInventoryTestData), DynamicDataSourceType.Property)]
    public void TestUpdate(List<Inventory> inventories, int idToUpdate, Inventory updatedInventory, bool expectedResult)
    {
        // Arrange
        foreach (Inventory inventory in inventories)
        {
            db.Inventories.Add(inventory);
            db.SaveChanges();
        }
        InventoriesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.updateInventory(idToUpdate, updatedInventory).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }
}