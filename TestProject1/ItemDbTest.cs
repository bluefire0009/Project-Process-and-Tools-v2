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
        for (int itemIterator = 0; itemIterator < result.Count; itemIterator++)
        {
            Assert.IsTrue(result[itemIterator].Equals(items[itemIterator]));
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
        Item? foundItem = storage.GetItem(soughtId).Result;

        // Assert
        bool actualResult = foundItem != null;
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> ItemAmountInWarehouseTestData => new List<object[]>
    {
        // Test with empty database
        new object[] { new List<Item> {}, new List<Location> {}, new List<Warehouse> {}, new List<Inventory> {}, "P00001", 1, 0},
        // Test with one item in database
        new object[]{
                    new List<Item> { new Item { Uid = "P00001", Code = "ITEM1", Description = "Test Item 1" } },
                    new List<Location> { new Location { Id = 1, WareHouseId = 1, Code = "LOC1", Name = "Location 1" } },
                    new List<Warehouse> { new Warehouse { Id = 1, Code = "WH1", Name = "Warehouse 1" } },
                    new List<Inventory> { new Inventory { Id = 1, ItemId = "P00001", total_on_hand = 15, InventoryLocations = new List<InventoryLocation> { new() {InventoryId = 1, LocationId = 1}} } },
                    "P00001", 1, 15},
        // Test with two items in database, and amount of one item is requested
        new object[]{
                    new List<Item> { new Item { Uid = "P00001", Code = "ITEM1", Description = "Test Item 1" }, new Item { Uid = "P00002", Code = "ITEM2", Description = "Test Item 2" } },
                    new List<Location> { new Location { Id = 1, WareHouseId = 1, Code = "LOC1", Name = "Location 1" }, new Location { Id = 2, WareHouseId = 1, Code = "LOC2", Name = "Location 2" } },
                    new List<Warehouse> { new Warehouse { Id = 1, Code = "WH1", Name = "Warehouse 1" } },
                    new List<Inventory> { new Inventory { Id = 1, ItemId = "P00001", total_on_hand = 15, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 1, LocationId = 1 } } }, new Inventory { Id = 2, ItemId = "P00002", total_on_hand = 10, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 2, LocationId = 2 } } } },
                    "P00001", 1, 15},
        // test with two items in database, each is in different warehouse
        new object[]{
                    new List<Item> { new Item { Uid = "P00001", Code = "ITEM1", Description = "Test Item 1" }, new Item { Uid = "P00002", Code = "ITEM2", Description = "Test Item 2" } },
                    new List<Location> { new Location { Id = 1, WareHouseId = 1, Code = "LOC1", Name = "Location 1" }, new Location { Id = 2, WareHouseId = 2, Code = "LOC2", Name = "Location 2" } },
                    new List<Warehouse> { new Warehouse { Id = 1, Code = "WH1", Name = "Warehouse 1" }, new Warehouse { Id = 2, Code = "WH2", Name = "Warehouse 2" } },
                    new List<Inventory> { new Inventory { Id = 1, ItemId = "P00001", total_on_hand = 15, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 1, LocationId = 1 } } }, new Inventory { Id = 2, ItemId = "P00002", total_on_hand = 10, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 2, LocationId = 2 } } } },
                    "P00001", 1, 15},
        // Test with three items in database, two items are in one warehouse, the last one in different warehouse
        new object[]{
                    new List<Item> { new Item { Uid = "P00001", Code = "ITEM1", Description = "Test Item 1" }, new Item { Uid = "P00002", Code = "ITEM2", Description = "Test Item 2" }, new Item { Uid = "P00003", Code = "ITEM3", Description = "Test Item 3" } },
                    new List<Location> { new Location { Id = 1, WareHouseId = 1, Code = "LOC1", Name = "Location 1" }, new Location { Id = 2, WareHouseId = 1, Code = "LOC2", Name = "Location 2" }, new Location { Id = 3, WareHouseId = 2, Code = "LOC3", Name = "Location 3" } },
                    new List<Warehouse> { new Warehouse { Id = 1, Code = "WH1", Name = "Warehouse 1" }, new Warehouse { Id = 2, Code = "WH2", Name = "Warehouse 2" } },
                    new List<Inventory> { new Inventory { Id = 1, ItemId = "P00001", total_on_hand = 15, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 1, LocationId = 1 } } }, new Inventory { Id = 2, ItemId = "P00002", total_on_hand = 20, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 2, LocationId = 2 } } }, new Inventory { Id = 3, ItemId = "P00003", total_on_hand = 10, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 3, LocationId = 3 } } } },
                    "P00002", 1, 20},
        // Test with two items in database, each in different warehouse, in one warehouse there two locations with the same item
        new object[]{
                    new List<Item> { new Item { Uid = "P00001", Code = "ITEM1", Description = "Test Item 1" }, new Item { Uid = "P00002", Code = "ITEM2", Description = "Test Item 2" } },
                    new List<Location> { new Location { Id = 1, WareHouseId = 1, Code = "LOC1", Name = "Location 1" }, new Location { Id = 2, WareHouseId = 1, Code = "LOC2", Name = "Location 2" }, new Location { Id = 3, WareHouseId = 2, Code = "LOC3", Name = "Location 3" } },
                    new List<Warehouse> { new Warehouse { Id = 1, Code = "WH1", Name = "Warehouse 1" }, new Warehouse { Id = 2, Code = "WH2", Name = "Warehouse 2" } },
                    new List<Inventory> { new Inventory { Id = 1, ItemId = "P00001", total_on_hand = 15, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 1, LocationId = 1 }, new() { InventoryId = 1, LocationId = 2 } } }, new Inventory { Id = 2, ItemId = "P00002", total_on_hand = 10, InventoryLocations = new List<InventoryLocation> { new() { InventoryId = 2, LocationId = 3 } } } },
                    "P00001", 1, 15}
    };
    [TestMethod]
    [DynamicData(nameof(ItemAmountInWarehouseTestData), DynamicDataSourceType.Property)]
    public void TestGetAmountInWarehouse(List<Item> items, List<Location> locations, List<Warehouse> warehouses, List<Inventory> inventories, string soughtItemId, int soughtWarehouseId, int expectedResult)
    {
        // Arrange
        addTestResourceToDB(items);
        addTestResourceToDB(warehouses);
        addTestResourceToDB(locations);
        addTestResourceToDB(inventories);
        
        ItemsDBStorage storage = new(db);

        // Act
        int actualResult = storage.GetItemAmountInWarehouse(soughtItemId, soughtWarehouseId).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> AddItemTestData => new List<object[]>
        {
            new object[] { null, false},
            new object[] { new Item(){Uid = ""}, false},
            new object[] { new Item(){Uid = "P00001", ItemLine=1, ItemGroup=1, ItemType=1, SupplierId=1}, true}
        };
    [TestMethod]
    [DynamicData(nameof(AddItemTestData), DynamicDataSourceType.Property)]
    public void TestAdd(Item item, bool expectedResult)
    {
        db.ItemLines.Add(new(){Id = 1});
        db.ItemGroups.Add(new(){Id = 1});
        db.ItemTypes.Add(new(){Id = 1});
        db.Suppliers.Add(new(){Id = 1});
        db.SaveChanges();

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
        db.ItemLines.Add(new(){Id = 1});
        db.ItemGroups.Add(new(){Id = 1});
        db.ItemTypes.Add(new(){Id = 1});
        db.Suppliers.Add(new(){Id = 1});
        db.SaveChanges();
        
        // Arrange
        Item i1 = new() {Uid = "P00001", ItemLine=1, ItemGroup=1, ItemType=1, SupplierId=1};
        Item i2 = new() {Uid = "P00001", ItemLine=1, ItemGroup=1, ItemType=1, SupplierId=1};
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

    private void addTestResourceToDB<T>(List<T> resources) where T : class
    {
        if (resources != null)
        {
            var testTable = db.Set<T>();
            foreach (T resource in resources)
            {
                testTable.Add(resource);
                db.SaveChanges();
            }
        }
    }
}