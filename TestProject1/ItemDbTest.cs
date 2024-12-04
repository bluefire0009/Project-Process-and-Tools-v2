using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class ItemDBTest
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

    
    public static IEnumerable<object[]> ItemsTestData => new List<object[]>
        {
            new object[] { new List<Item> {}},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}}},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}, new Item() {Uid = "P00002"}}}
        };
    [TestMethod]
    [DynamicData(nameof(ItemsTestData), DynamicDataSourceType.Property)]
    public void TestGetAll(List<Item> items)
    {
        // Arrange
        foreach (Item item in items)
        {
            db.Items.Add(item);
            db.SaveChanges();
        }
        ItemsDBStorage storage = new(db);

        // Act
        List<Item> result = storage.GetItems(0, 100).Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == items.Count);
        for (int itemIterator = 0; itemIterator < result.Count; itemIterator++)
        {
            Assert.IsTrue(result[itemIterator].Equals(items[itemIterator]));
        }
    }

    public static IEnumerable<object[]> PaginationTestData => new List<object[]>
    {
        new object[] { 0, 0, 10, 0},
        new object[] { 10, 0, 100, 10},
        new object[] { 10, 5, 10, 5},
        new object[] { 10, 10, 10, 0},
        new object[] { 10, 0, 0, 0},
        new object[] { 10, -1, 5, 5},
        new object[] { 30, 10, 10, 10},
        new object[] { 10, 10, -1, 0}
    };
    [TestMethod]
    [DynamicData(nameof(PaginationTestData), DynamicDataSourceType.Property)]
    public void TestGetPagination(int AmountItems, int offset, int limit, int expectedAmount)
    {
        // Arrange
        for (int i = 0; i < AmountItems; i++)
        {
            Item item = new() {Uid = "P"+i};
            db.Items.Add(item);
            db.SaveChanges();
        }

        ItemsDBStorage storage = new(db);

        // Act
        List<Item> result = storage.GetItems(offset, limit).Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == expectedAmount);
    }

    public static IEnumerable<object[]> SpecificItemTestData => new List<object[]>
        {
            new object[] { new List<Item> {}, "P00001", false},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}}, "P00002", false},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}}, "P00001", true},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}, new Item(){Uid = "P00002"}}, "P00002", true}
        };
    [TestMethod]
    [DynamicData(nameof(SpecificItemTestData), DynamicDataSourceType.Property)]
    public void TestGetSpecific(List<Item> items, string soughtId, bool expectedResult)
    {
        // Arrange
        foreach (Item item in items)
        {
            db.Items.Add(item);
            db.SaveChanges();
        }
        ItemsDBStorage storage = new(db);

        // Act
        Item? foundItem = storage.GetItem(soughtId).Result;

        // Assert
        bool actualResult = foundItem != null;
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> AddItemTestData => new List<object[]>
        {
            new object[] { null, false},
            new object[] { new Item(){Uid = ""}, false},
            new object[] { new Item(){Uid = "P00001"}, true}
        };
    [TestMethod]
    [DynamicData(nameof(AddItemTestData), DynamicDataSourceType.Property)]
    public void TestAdd(Item item, bool expectedResult)
    {
        // Arrange
        ItemsDBStorage storage = new(db);

        // Act
        bool actualResult = storage.AddItem(item).Result;

        // Assert
        Assert.IsTrue(actualResult.Equals(expectedResult));
        if (expectedResult == true)
            Assert.IsTrue(db.Items.Contains(item));
        if (expectedResult == false)
            Assert.IsTrue(!db.Items.Contains(item));
    }

    [TestMethod]
    public void TestAddSameIdTwice()
    {
        // Arrange
        Item i1 = new() { Uid = "P00001" };
        Item i2 = new() { Uid = "P00001" };
        ItemsDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.AddItem(i1).Result;
        bool secondAdd = storage.AddItem(i2).Result;

        // Assert
        Assert.IsTrue(firstAdd == true);
        Assert.IsTrue(secondAdd == false);
    }

    public static IEnumerable<object[]> RemoveItemTestData => new List<object[]>
        {
            new object[] { new List<Item> {}, "P00001", false},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}}, "", false},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}}, "P00000", false},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}}, "P00003", false},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}}, "P00001", true},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}, new Item(){Uid = "P00002"}}, "P00002", true}
        };
    [TestMethod]
    [DynamicData(nameof(RemoveItemTestData), DynamicDataSourceType.Property)]
    public void TestRemove(List<Item> items, string idToRemove, bool expectedResult)
    {
        // Arrange
        foreach (Item item in items)
        {
            db.Items.Add(item);
            db.SaveChanges();
        }
        ItemsDBStorage storage = new(db);

        // Act
        bool actualResult = storage.DeleteItem(idToRemove).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if (expectedResult == true)
            Assert.IsTrue(db.Items.Count() == items.Count -1);
        if (expectedResult == false)
            Assert.IsTrue(db.Items.Count() == items.Count);
    }

    [TestMethod]
    public void TestRemoveSameTwice()
    {
        // Arrange
        Item i1 = new() { Uid = "P00001" };
        db.Items.Add(i1);
        db.SaveChanges();
        ItemsDBStorage storage = new(db);


        // Act
        bool firstRemove = storage.DeleteItem(i1.Uid).Result;
        bool secondRemove = storage.DeleteItem(i1.Uid).Result;

        // Assert
        Assert.IsTrue(firstRemove == true);
        Assert.IsTrue(secondRemove == false);
    }

    public static IEnumerable<object[]> UpdateItemTestData => new List<object[]>
        {
            new object[] { new List<Item> {}, "P00002", new Item(){Uid = "P00001"},false},
            new object[] { new List<Item> {}, "P00001", null,false},
            new object[] { new List<Item> {}, "", new Item(){Uid = "P00001"},false},
            new object[] { new List<Item> {}, "P00001", new Item(){Uid = "P00001"},false},
            new object[] { new List<Item> {new Item(){Uid = "P00001"}}, "P00001", new Item(){Uid = "P00002"}, false},
            new object[] { new List<Item> {new Item(){Uid = "P00001", Code="123"}}, "P00001", new Item(){Uid = "P00001", Code = "ABC"}, true},
        };
    [TestMethod]
    [DynamicData(nameof(UpdateItemTestData), DynamicDataSourceType.Property)]
    public void TestUpdate(List<Item> items, string idToUpdate, Item updatedItem, bool expectedResult)
    {
        // Arrange
        foreach (Item item in items)
        {
            db.Items.Add(item);
            db.SaveChanges();
        }
        ItemsDBStorage storage = new(db);

        // Act
        bool actualResult = storage.UpdateItem(idToUpdate, updatedItem).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if (expectedResult == true)
            Assert.IsTrue(db.Items.Contains(updatedItem));
        if (expectedResult == false)
            Assert.IsTrue(!db.Items.Contains(updatedItem));
    }

    public static IEnumerable<object[]> GetItemInventoriesTestData => new List<object[]>
        {
            new object[] { new List<Item> {new(){Uid = "P00001"}}, new List<Inventory> {new(){Id = 1, ItemId = "P00001"}, new(){Id = 2, ItemId = "P00001"}, new(){Id = 3, ItemId = "P00001"}}, "P00001"},
            new object[] { new List<Item> {new(){Uid = "P00001"}}, null, ""},
            new object[] { new List<Item> {new(){Uid = "P00001"}}, null, ""},
            new object[] { new List<Item> {new(){Uid = "P00002"}}, new List<Inventory> {new(){Id = 1, ItemId = "P00001"}, new(){Id = 2, ItemId = "P00001"}, new(){Id = 3, ItemId = "P00001"}}, "P00002"},
        };
    [TestMethod]
    [DynamicData(nameof(GetItemInventoriesTestData), DynamicDataSourceType.Property)]
    public void TestGetItemInventories(List<Item> items, List<Inventory> inventories, string soughtId)
    {
        // Arrange
        foreach (Item item in items)
        {
            db.Items.Add(item);
            db.SaveChanges();
        }
        if (inventories != null)
        {
            foreach (Inventory inventory in inventories)
            {
                db.Inventories.Add(inventory);
                db.SaveChanges();
            }
        }
        ItemsDBStorage storage = new(db);

        // Act
        List<Inventory> result = storage.GetItemInventory(soughtId).Result.ToList();


        // Assert
        for (int inventoriesIterator = 0; inventoriesIterator < result.Count; inventoriesIterator++)
        {
            Assert.IsTrue(result[inventoriesIterator].Id == inventories[inventoriesIterator].Id);
            Assert.IsTrue(result[inventoriesIterator].ItemId == inventories[inventoriesIterator].ItemId);
            Assert.IsTrue(result[inventoriesIterator].Description == inventories[inventoriesIterator].Description);
            Assert.IsTrue(result[inventoriesIterator].ItemReference == inventories[inventoriesIterator].ItemReference);
        }
        if (result.Count == 0 && result != null && inventories != null)
            Assert.IsTrue(inventories.Where(_ => _.ItemId == soughtId) != null);
        if (result == null)
            Assert.IsTrue(inventories.Where(_ => _.ItemId == soughtId) == null);
    }
}