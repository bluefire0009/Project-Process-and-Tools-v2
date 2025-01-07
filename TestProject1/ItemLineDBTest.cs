using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class ItemLineDBTest
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

    
    public static IEnumerable<object[]> ItemLinesTestData => new List<object[]>
        {
            new object[] { new List<ItemLine> {}},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}}},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}, new ItemLine() {Id = 2}}}
        };
    [TestMethod]
    [DynamicData(nameof(ItemLinesTestData), DynamicDataSourceType.Property)]
    public void TestGetAll(List<ItemLine> itemlines)
    {
        // Arrange
        foreach (ItemLine itemline in itemlines)
        {
            db.ItemLines.Add(itemline);
            db.SaveChanges();
        }
        ItemLinesDBStorage storage = new(db);

        // Act
        List<ItemLine> result = storage.GetItemLines(0, 100).Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == itemlines.Count);
        for (int itemlineIterator = 0; itemlineIterator < result.Count; itemlineIterator++)
        {
            Assert.IsTrue(result[itemlineIterator].Equals(itemlines[itemlineIterator]));
        }
    }

    public static IEnumerable<object[]> SpecificItemLineTestData => new List<object[]>
        {
            new object[] { new List<ItemLine> {}, 1, false},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}}, 2, false},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}}, 1, true},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}, new ItemLine(){Id = 2}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(SpecificItemLineTestData), DynamicDataSourceType.Property)]
    public void TestGetSpecific(List<ItemLine> itemlines, int soughtId, bool expectedResult)
    {
        // Arrange
        foreach (ItemLine itemline in itemlines)
        {
            db.ItemLines.Add(itemline);
            db.SaveChanges();
        }
        ItemLinesDBStorage storage = new(db);

        // Act
        ItemLine? foundItemLine = storage.GetItemLine(soughtId).Result;

        // Assert
        bool actualResult = foundItemLine != null;
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> AddItemLineTestData => new List<object[]>
        {
            new object[] { null, false},
            new object[] { new ItemLine(){Id = -1}, false},
            new object[] { new ItemLine(){Id = 1}, true}
        };
    [TestMethod]
    [DynamicData(nameof(AddItemLineTestData), DynamicDataSourceType.Property)]
    public void TestAdd(ItemLine itemline, bool expectedResult)
    {
        // Arrange
        ItemLinesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.AddItemLine(itemline).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if (expectedResult == true)
            Assert.IsTrue(db.ItemLines.Contains(itemline));
        if (expectedResult == false)
            Assert.IsTrue(!db.ItemLines.Contains(itemline));
    }

    [TestMethod]
    public void TestAddSameIdTwice()
    {
        // Arrange
        ItemLine i1 = new() { Id = 1 };
        ItemLine i2 = new() { Id = 1 };
        ItemLinesDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.AddItemLine(i1).Result;
        bool secondAdd = storage.AddItemLine(i2).Result;

        // Assert
        Assert.IsTrue(firstAdd == true);
        Assert.IsTrue(secondAdd == false);
    }

    public static IEnumerable<object[]> RemoveItemLineTestData => new List<object[]>
        {
            new object[] { new List<ItemLine> {}, 1, false},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}}, -1, false},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}}, 0, false},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}}, 3, false},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}}, 1, true},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}, new ItemLine(){Id = 2}}, 2, true},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}, new ItemLine(){Id = 2, IsDeleted=true}}, 3, false},
            new object[] { new List<ItemLine> { new ItemLine(){Id = 1}, new ItemLine(){Id = 2, IsDeleted=true}}, 1, true}
        };
    [TestMethod]
    [DynamicData(nameof(RemoveItemLineTestData), DynamicDataSourceType.Property)]
    public void TestRemove(List<ItemLine> itemlines, int idToRemove, bool expectedResult)
    {
        int oldCount = itemlines.Where(_ => !_.IsDeleted).Count();
        // Arrange
        foreach (ItemLine itemline in itemlines)
        {
            db.ItemLines.Add(itemline);
            db.SaveChanges();
        }
        ItemLinesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.DeleteItemLine(idToRemove).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if (expectedResult == true)
            Assert.IsTrue(db.ItemLines.Where(_ => !_.IsDeleted).Count() == oldCount -1);
        if (expectedResult == false)
            Assert.IsTrue(db.ItemLines.Where(_ => !_.IsDeleted).Count() == oldCount);
    }

    [TestMethod]
    public void TestRemoveSameTwice()
    {
        // Arrange
        ItemLine i1 = new() { Id = 1 };
        db.ItemLines.Add(i1);
        db.SaveChanges();
        ItemLinesDBStorage storage = new(db);


        // Act
        bool firstRemove = storage.DeleteItemLine(i1.Id).Result;
        bool secondRemove = storage.DeleteItemLine(i1.Id).Result;

        // Assert
        Assert.IsTrue(firstRemove == true);
        Assert.IsTrue(secondRemove == false);
    }

    public static IEnumerable<object[]> UpdateItemLineTestData => new List<object[]>
        {
            new object[] { new List<ItemLine> {}, 2, new ItemLine(){Id = 1},false},
            new object[] { new List<ItemLine> {}, 1, null,false},
            new object[] { new List<ItemLine> {}, -1, new ItemLine(){Id = 1},false},
            new object[] { new List<ItemLine> {}, 1, new ItemLine(){Id = 1},false},
            new object[] { new List<ItemLine> {new ItemLine(){Id = 1}}, 1, new ItemLine(){Id = 2}, false},
            new object[] { new List<ItemLine> {new ItemLine(){Id = 1, Description="123"}}, 1, new ItemLine(){Id = 1, Description = "ABC"}, true},
        };
    [TestMethod]
    [DynamicData(nameof(UpdateItemLineTestData), DynamicDataSourceType.Property)]
    public void TestUpdate(List<ItemLine> itemlines, int idToUpdate, ItemLine updatedItemLine, bool expectedResult)
    {
        // Arrange
        foreach (ItemLine itemline in itemlines)
        {
            db.ItemLines.Add(itemline);
            db.SaveChanges();
        }
        ItemLinesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.UpdateItemLine(idToUpdate, updatedItemLine).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if (expectedResult == true)
            Assert.IsTrue(db.ItemLines.Contains(updatedItemLine));
        if (expectedResult == false)
            Assert.IsTrue(!db.ItemLines.Contains(updatedItemLine));
    }

    public static IEnumerable<object[]> GetItemLineInventoriesTestData => new List<object[]>
        {
            new object[] { new List<ItemLine> {new(){Id = 1}}, new List<Item> {new(){Uid = "P00001", ItemLine=1}, new(){Uid = "P00002", ItemLine=1}, new(){Uid = "P00003", ItemLine=1}}, 1},
            new object[] { new List<ItemLine> {new(){Id = 1}}, null, 0},
            new object[] { new List<ItemLine> {new(){Id = 1}}, null, -1},
            new object[] { new List<ItemLine> {new(){Id = 2}}, new List<Item> {new(){Uid = "P00001", ItemLine=1}, new(){Uid = "P00002", SupplierId = 1}, new(){Uid = "P00003", SupplierId = 1}}, 2},
        };
    [TestMethod]
    [DynamicData(nameof(GetItemLineInventoriesTestData), DynamicDataSourceType.Property)]
    public void TestGetItemLineItems(List<ItemLine> itemlines, List<Item> items, int soughtId)
    {
        // Arrange
        foreach (ItemLine itemline in itemlines)
        {
            db.ItemLines.Add(itemline);
            db.SaveChanges();
        }
        if (items != null)
        {
            foreach (Item item in items)
            {
                db.Items.Add(item);
                db.SaveChanges();
            }
        }
        ItemLinesDBStorage storage = new(db);

        // Act
        List<Item> result = storage.GetItemLineItems(soughtId).Result.ToList();


        // Assert
        for (int itemsIterator = 0; itemsIterator < result.Count; itemsIterator++)
        {
            Assert.IsTrue(result[itemsIterator]== items[itemsIterator]);
        if (result.Count == 0 && result != null)
            Assert.IsTrue(items != null);
        if (result == null)
            Assert.IsTrue(items == null);
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
    public void TestGetPagination(int AmountItemLines, int offset, int limit, int expectedAmount)
    {
        // Arrange
        for (int i = 0; i < AmountItemLines; i++)
        {
            ItemLine itemLine = new() {Id = i+1};
            db.ItemLines.Add(itemLine);
            db.SaveChanges();
        }

        ItemLinesDBStorage storage = new(db);

        // Act
        List<ItemLine> result = storage.GetItemLines(offset, limit).Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == expectedAmount);
    }

}