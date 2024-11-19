using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class ItemGroupDBStorageTest
{
    private DatabaseContext db;

    [TestInitialize]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        db = new DatabaseContext(options);
    }

    public static IEnumerable<object[]> ItemGroupsTestData => new List<object[]>
    {
        new object[] { new List<ItemGroup> {} },
        new object[] { new List<ItemGroup> { new ItemGroup() } },
        new object[] { new List<ItemGroup> { new ItemGroup(), new ItemGroup() } }
    };

    [TestMethod]
    [DynamicData(nameof(ItemGroupsTestData), DynamicDataSourceType.Property)]
    public void TestGetAllItemGroups(List<ItemGroup> itemGroups)
    {
        // Arrange
        foreach (ItemGroup itemGroup in itemGroups)
        {
            db.ItemGroups.Add(itemGroup);
            db.SaveChanges();
        }
        ItemGroupDBStorage storage = new(db);

        // Act
        List<ItemGroup> result = storage.getItemGroups().Result.ToList();

        // Assert
        Assert.AreEqual(itemGroups.Count, result.Count);
    }

    public static IEnumerable<object[]> SpecificItemGroupTestData => new List<object[]>
    {
        new object[] { new List<ItemGroup> {}, 1, false },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 } }, 2, false },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 } }, 1, true },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 }, new ItemGroup() { Id = 2 } }, 2, true }
    };

    [TestMethod]
    [DynamicData(nameof(SpecificItemGroupTestData), DynamicDataSourceType.Property)]
    public void TestGetSpecificItemGroup(List<ItemGroup> itemGroups, int soughtId, bool expectedResult)
    {
        // Arrange
        foreach (ItemGroup itemGroup in itemGroups)
        {
            db.ItemGroups.Add(itemGroup);
            db.SaveChanges();
        }
        ItemGroupDBStorage storage = new(db);

        // Act
        ItemGroup? foundItemGroup = storage.getItemGroup(soughtId).Result;

        // Assert
        Assert.AreEqual(expectedResult, foundItemGroup != null);
    }

    public static IEnumerable<object[]> AddItemGroupTestData => new List<object[]>
    {
        new object[] { null, false },
        new object[] { new ItemGroup() { Id = -1 }, false },
        new object[] { new ItemGroup() { Id = 0 }, false },
        new object[] { new ItemGroup() { Id = 1 }, true }
    };

    [TestMethod]
    [DynamicData(nameof(AddItemGroupTestData), DynamicDataSourceType.Property)]
    public void TestAddItemGroup(ItemGroup itemGroup, bool expectedResult)
    {
        // Arrange
        ItemGroupDBStorage storage = new(db);

        // Act
        bool actualResult = storage.addItemGroup(itemGroup).Result;

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void TestAddItemGroupWithDuplicateId()
    {
        // Arrange
        ItemGroup g1 = new() { Id = 1 };
        ItemGroup g2 = new() { Id = 1 };
        ItemGroupDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.addItemGroup(g1).Result;
        bool secondAdd = storage.addItemGroup(g2).Result;

        // Assert
        Assert.IsTrue(firstAdd);
        Assert.IsFalse(secondAdd);
    }

    public static IEnumerable<object[]> UpdateItemGroupTestData => new List<object[]>
    {
        new object[] { new List<ItemGroup> {}, 2, new ItemGroup() { Id = 1 }, false },
        new object[] { new List<ItemGroup> {}, 1, null, false },
        new object[] { new List<ItemGroup> {}, 0, new ItemGroup() { Id = 1 }, false },
        new object[] { new List<ItemGroup> {}, -1, new ItemGroup() { Id = 1 }, false },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 } }, 1, new ItemGroup() { Id = 1 }, true },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 } }, 1, new ItemGroup() { Id = 2 }, true }
    };

    [TestMethod]
    [DynamicData(nameof(UpdateItemGroupTestData), DynamicDataSourceType.Property)]
    public void TestUpdateItemGroup(List<ItemGroup> itemGroups, int idToUpdate, ItemGroup updatedItemGroup, bool expectedResult)
    {
        // Arrange
        foreach (ItemGroup itemGroup in itemGroups)
        {
            db.ItemGroups.Add(itemGroup);
            db.SaveChanges();
        }
        ItemGroupDBStorage storage = new(db);

        // Act
        bool actualResult = storage.updateItemGroup(idToUpdate, updatedItemGroup).Result;

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }

    public static IEnumerable<object[]> DeleteItemGroupTestData => new List<object[]>
    {
        new object[] { new List<ItemGroup> {}, 1, false },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 } }, 0, false },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 } }, -1, false },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 } }, 2, false },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 } }, 1, true },
        new object[] { new List<ItemGroup> { new ItemGroup() { Id = 1 }, new ItemGroup() { Id = 2 } }, 2, true }
    };

    [TestMethod]
    [DynamicData(nameof(DeleteItemGroupTestData), DynamicDataSourceType.Property)]
    public void TestDeleteItemGroup(List<ItemGroup> itemGroups, int idToDelete, bool expectedResult)
    {
        // Arrange
        foreach (ItemGroup itemGroup in itemGroups)
        {
            db.ItemGroups.Add(itemGroup);
            db.SaveChanges();
        }
        ItemGroupDBStorage storage = new(db);

        // Act
        bool actualResult = storage.deleteItemGroup(idToDelete).Result;

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }

    public static IEnumerable<object[]> ItemGroupItemsTestData => new List<object[]>
    {
        new object[] { 1, new List<Item> { new Item { Uid = "1", ItemGroup = 1 } }, 1 },
        new object[] { 2, new List<Item> { new Item { Uid = "1", ItemGroup = 1 } }, 0 },
        new object[] { 1, new List<Item> { }, 0 }
    };

    [TestMethod]
    [DynamicData(nameof(ItemGroupItemsTestData), DynamicDataSourceType.Property)]
    public void TestGetItemGroupItems(int itemGroupId, List<Item> items, int expectedItemCount)
    {
        // Arrange
        foreach (Item item in items)
        {
            db.Items.Add(item);
        }
        db.SaveChanges();
        ItemGroupDBStorage storage = new(db);

        // Act
        var result = storage.getItemGroupItems(itemGroupId);

        // Assert
        Assert.AreEqual(expectedItemCount, result?.Count() ?? 0);
    }
}