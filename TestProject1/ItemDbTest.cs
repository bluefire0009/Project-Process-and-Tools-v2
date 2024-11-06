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
        List<Item> result = storage.GetItems().Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == items.Count);
        for (int supplierIterator = 0; supplierIterator < result.Count; supplierIterator++)
        {
            Assert.IsTrue(result[supplierIterator].Equals(items[supplierIterator]));
        }
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
        Item? foundSupplier = storage.GetItem(soughtId).Result;

        // Assert
        bool actualResult = foundSupplier != null;
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
        Assert.IsTrue(actualResult == expectedResult);
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
            new object[] { new List<Item> {}, 1, false},
            new object[] { new List<Item> { new Item(){Uid = "P00001"}}, "P00001", false},
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
    }

}