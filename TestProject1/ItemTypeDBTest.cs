using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class ItemTypeDBTest
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

    
    public static IEnumerable<object[]> ItemTypesTestData => new List<object[]>
        {
            new object[] { new List<ItemType> {}},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}}},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}, new ItemType() {Id = 2}}}
        };
    [TestMethod]
    [DynamicData(nameof(ItemTypesTestData), DynamicDataSourceType.Property)]
    public void TestGetAll(List<ItemType> itemtypes)
    {
        // Arrange
        foreach (ItemType itemtype in itemtypes)
        {
            db.ItemTypes.Add(itemtype);
            db.SaveChanges();
        }
        ItemTypesDBStorage storage = new(db);

        // Act
        List<ItemType> result = storage.GetItemTypes().Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == itemtypes.Count);
        for (int itemtypeIterator = 0; itemtypeIterator < result.Count; itemtypeIterator++)
        {
            Assert.IsTrue(result[itemtypeIterator].Equals(itemtypes[itemtypeIterator]));
        }
    }

    public static IEnumerable<object[]> SpecificItemTypeTestData => new List<object[]>
        {
            new object[] { new List<ItemType> {}, 1, false},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}}, 2, false},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}}, 1, true},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}, new ItemType(){Id = 2}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(SpecificItemTypeTestData), DynamicDataSourceType.Property)]
    public void TestGetSpecific(List<ItemType> itemtypes, int soughtId, bool expectedResult)
    {
        // Arrange
        foreach (ItemType itemtype in itemtypes)
        {
            db.ItemTypes.Add(itemtype);
            db.SaveChanges();
        }
        ItemTypesDBStorage storage = new(db);

        // Act
        ItemType? foundItemType = storage.GetItemType(soughtId).Result;

        // Assert
        bool actualResult = foundItemType != null;
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> AddItemTypeTestData => new List<object[]>
        {
            new object[] { null, false},
            new object[] { new ItemType(){Id = -1}, false},
            new object[] { new ItemType(){Id = 1}, true}
        };
    [TestMethod]
    [DynamicData(nameof(AddItemTypeTestData), DynamicDataSourceType.Property)]
    public void TestAdd(ItemType itemtype, bool expectedResult)
    {
        // Arrange
        ItemTypesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.AddItemType(itemtype).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if(expectedResult == true)
            Assert.IsTrue(db.ItemTypes.Contains(itemtype));
        if(expectedResult == false)
            Assert.IsTrue(!db.ItemTypes.Contains(itemtype));
    }

    [TestMethod]
    public void TestAddSameIdTwice()
    {
        // Arrange
        ItemType i1 = new() { Id = 1 };
        ItemType i2 = new() { Id = 1 };
        ItemTypesDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.AddItemType(i1).Result;
        bool secondAdd = storage.AddItemType(i2).Result;

        // Assert
        Assert.IsTrue(firstAdd == true);
        Assert.IsTrue(secondAdd == false);
        Assert.IsTrue(db.ItemTypes.Count() == 1);
    }

    public static IEnumerable<object[]> RemoveItemTypeTestData => new List<object[]>
        {
            new object[] { new List<ItemType> {}, 1, false},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}}, -1, false},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}}, 0, false},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}}, 3, false},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}}, 1, true},
            new object[] { new List<ItemType> { new ItemType(){Id = 1}, new ItemType(){Id = 2}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(RemoveItemTypeTestData), DynamicDataSourceType.Property)]
    public void TestRemove(List<ItemType> itemtypes, int idToRemove, bool expectedResult)
    {
        // Arrange
        foreach (ItemType itemtype in itemtypes)
        {
            db.ItemTypes.Add(itemtype);
            db.SaveChanges();
        }
        ItemTypesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.DeleteItemType(idToRemove).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if(expectedResult == true)
            Assert.IsTrue(db.ItemTypes.Count() == itemtypes.Count -1);
        if(expectedResult == false)
            Assert.IsTrue(db.ItemTypes.Count() == itemtypes.Count);
    }

    [TestMethod]
    public void TestRemoveSameTwice()
    {
        // Arrange
        ItemType i1 = new() { Id = 1 };
        db.ItemTypes.Add(i1);
        db.SaveChanges();
        ItemTypesDBStorage storage = new(db);


        // Act
        bool firstRemove = storage.DeleteItemType(i1.Id).Result;
        bool secondRemove = storage.DeleteItemType(i1.Id).Result;

        // Assert
        Assert.IsTrue(firstRemove == true);
        Assert.IsTrue(secondRemove == false);
    }

    public static IEnumerable<object[]> UpdateItemTypeTestData => new List<object[]>
        {
            new object[] { new List<ItemType> {}, 2, new ItemType(){Id = 1},false},
            new object[] { new List<ItemType> {}, 1, null,false},
            new object[] { new List<ItemType> {}, -1, new ItemType(){Id = 1},false},
            new object[] { new List<ItemType> {}, 1, new ItemType(){Id = 1},false},
            new object[] { new List<ItemType> {new ItemType(){Id = 1}}, 1, new ItemType(){Id = 2}, false},
            new object[] { new List<ItemType> {new ItemType(){Id = 1, Description="123"}}, 1, new ItemType(){Id = 1, Description = "ABC"}, true},
        };
    [TestMethod]
    [DynamicData(nameof(UpdateItemTypeTestData), DynamicDataSourceType.Property)]
    public void TestUpdate(List<ItemType> itemtypes, int idToUpdate, ItemType updatedItemType, bool expectedResult)
    {
        // Arrange
        foreach (ItemType itemtype in itemtypes)
        {
            db.ItemTypes.Add(itemtype);
            db.SaveChanges();
        }
        ItemTypesDBStorage storage = new(db);

        // Act
        bool actualResult = storage.UpdateItemType(idToUpdate, updatedItemType).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if(expectedResult == true)
            Assert.IsTrue(db.ItemTypes.Contains(updatedItemType));
        if(expectedResult == false)
            Assert.IsTrue(!db.ItemTypes.Contains(updatedItemType));
    }

    public static IEnumerable<object[]> GetItemTypeInventoriesTestData => new List<object[]>
        {
            new object[] { new List<ItemType> {new(){Id = 1}}, new List<Item> {new(){Uid = "P00001", ItemType=1}, new(){Uid = "P00002", ItemType=1}, new(){Uid = "P00003", ItemType=1}}, 1},
            new object[] { new List<ItemType> {new(){Id = 1}}, null, 0},
            new object[] { new List<ItemType> {new(){Id = 1}}, null, -1},
            new object[] { new List<ItemType> {new(){Id = 2}}, new List<Item> {new(){Uid = "P00001", ItemType=1}, new(){Uid = "P00002", SupplierId = 1}, new(){Uid = "P00003", SupplierId = 1}}, 2},
        };
    [TestMethod]
    [DynamicData(nameof(GetItemTypeInventoriesTestData), DynamicDataSourceType.Property)]
    public void TestGetItemTypeItems(List<ItemType> itemtypes, List<Item> items, int soughtId)
    {
        // Arrange
        foreach (ItemType itemtype in itemtypes)
        {
            db.ItemTypes.Add(itemtype);
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
        ItemTypesDBStorage storage = new(db);

        // Act
        List<Item> result = storage.GetItemTypeItems(soughtId).Result.ToList();


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
}