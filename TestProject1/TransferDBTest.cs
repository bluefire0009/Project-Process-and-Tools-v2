using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;
using System.Reflection;
using System.Collections;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class TransferDBTest
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

    public static IEnumerable<object[]> TransfersTestData => new List<object[]>
        {
            new object[] { new List<Transfer> {}},
            new object[] { new List<Transfer> { new Transfer()}},
            new object[] { new List<Transfer> { new Transfer(), new Transfer() }}
        };
    [TestMethod]
    [DynamicData(nameof(TransfersTestData), DynamicDataSourceType.Property)]
    public void TestGetAll(List<Transfer> transfers)
    {
        // Arrange
        addTestResourceToDB(transfers);
        TransferDBStorage storage = new(db);

        // Act
        List<Transfer> result = storage.getTransfers().Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == transfers.Count);
        for (int transferIterator = 0; transferIterator < result.Count; transferIterator++)
        {
            Assert.IsTrue(result[transferIterator].Id == transfers[transferIterator].Id);
            Assert.IsTrue(result[transferIterator].Reference == transfers[transferIterator].Reference);
            Assert.IsTrue(result[transferIterator].TransferFrom == transfers[transferIterator].TransferFrom);
            Assert.IsTrue(result[transferIterator].TransferTo == transfers[transferIterator].TransferTo);
            Assert.IsTrue(result[transferIterator].TransferStatus == transfers[transferIterator].TransferStatus);
            for (int itemIterator = 0; itemIterator < result[transferIterator].Items.Count; itemIterator++)
            {
                Assert.IsTrue(result[transferIterator].Items[itemIterator].ItemUid == transfers[transferIterator].Items[itemIterator].ItemUid);
                Assert.IsTrue(result[transferIterator].Items[itemIterator].TransferId == transfers[transferIterator].Items[itemIterator].TransferId);
                Assert.IsTrue(result[transferIterator].Items[itemIterator].Amount == transfers[transferIterator].Items[itemIterator].Amount);
            }
            Assert.IsTrue(result[transferIterator].CreatedAt == transfers[transferIterator].CreatedAt);
            Assert.IsTrue(result[transferIterator].UpdatedAt == transfers[transferIterator].UpdatedAt);
        }
    }

    public static IEnumerable<object[]> SpecificTransfersTestData => new List<object[]>
        {
            new object[] { new List<Transfer> {}, 1, false},
            new object[] { new List<Transfer> { new Transfer(){Id = 1}}, 2, false},
            new object[] { new List<Transfer> { new Transfer(){Id = 1}}, 1, true},
            new object[] { new List<Transfer> { new Transfer(){Id = 1}, new Transfer(){Id = 2}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(SpecificTransfersTestData), DynamicDataSourceType.Property)]
    public void TestGetSpecific(List<Transfer> transfers, int soughtId, bool expectedResult)
    {
        // Arrange
        addTestResourceToDB(transfers);
        TransferDBStorage storage = new(db);

        // Act
        Transfer? foundTransfer = storage.getTransfer(soughtId).Result;

        // Assert
        bool actualResult = foundTransfer != null;
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> AddTransferTestData => new List<object[]>
        {
            new object[] { null, null, null, false},
            new object[] { null, null, null, false},
            new object[] { null, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = "1"}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = "1"}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = "1"}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = "1"}}}, false},
            new object[] { null, null, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = "0"}}}, false},
            new object[] { null, null, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = "0"}}}, false},
            new object[] { null, null, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = "0"}}}, false},
            new object[] { null, null, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = "0"}}}, false},
            new object[] { null, null, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = "-1"}}}, false},
            new object[] { null, null, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = "-1"}}}, false},
            new object[] { null, null, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = "-1"}}}, false},
            new object[] { null, null, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = "-1"}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, Items = {new() {TransferId = 1, ItemUid = "1"}, new(){TransferId = 1, ItemUid = "1"}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, Items = {new() {TransferId = 1, ItemUid = "1"}, new(){TransferId = 1, ItemUid = "1"}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, TransferFrom = 3, TransferTo = 4, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, TransferFrom = 3, TransferTo = 4, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, TransferFrom = 4, TransferTo = 2, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, TransferFrom = 4, TransferTo = 2, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, TransferFrom = 2, TransferTo = 4, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, TransferFrom = 2, TransferTo = 2, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, TransferFrom = 2, TransferTo = 1, Items = {new(){TransferId = 2, ItemUid = "1"}}}, false},

            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}}, new Transfer(){Id = 1, TransferFrom = 1, TransferTo = 2, Items = {new(){TransferId = 1, ItemUid = "1"}}}, true},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = "1"}, new(){Uid = "2"}}, new Transfer(){Id = 1, TransferFrom = 1, TransferTo = 2, Items = {new(){TransferId = 1, ItemUid = "1"}, new(){TransferId = 1, ItemUid = "2"}}}, true}
        };
    [TestMethod]
    [DynamicData(nameof(AddTransferTestData), DynamicDataSourceType.Property)]
    public void TestAdd(List<Location> locations, List<Item> items, Transfer transfer, bool expectedResult)
    {
        // Arrange
        addTestResourceToDB(locations);
        addTestResourceToDB(items);
        TransferDBStorage storage = new(db);

        // Act
        bool actualResult = storage.addTransfer(transfer).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        if (transfer != null)
        {
            if (expectedResult == false)
            {
                Assert.IsTrue(!db.Transfers.Select(t => t.Id).Contains(transfer.Id));
            }
            if (expectedResult == true)
            {
                foreach (TransferItem item in transfer.Items)
                {
                    Assert.IsTrue(db.TransferItems.Contains(item));
                }
                Assert.IsTrue(db.Transfers.Select(t => t.Id).Contains(transfer.Id));
                Assert.IsTrue(transfer.Items.Count == db.TransferItems.Count());
            }
        }
    }

    [TestMethod]
    public void TestAddSameIdTwice()
    {
        // Arrange
        Location l1 = new() { Id = 1 };
        Location l2 = new() { Id = 2 };
        db.Locations.Add(l1);
        db.Locations.Add(l2);
        db.SaveChanges();

        Transfer t1 = new() { Id = 1, TransferFrom = l1.Id, TransferTo = l2.Id };
        TransferDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.addTransfer(t1).Result;
        bool secondAdd = storage.addTransfer(t1).Result;

        // Assert
        Assert.IsTrue(firstAdd == true);
        Assert.IsTrue(secondAdd == false);
    }

    public static IEnumerable<object[]> RemoveTransferTestData => new List<object[]>
        {
            new object[] { null, new List<Transfer> {}, 1, false},
            new object[] { null, new List<Transfer> { new Transfer(){Id = 1}}, 0, false},
            new object[] { null, new List<Transfer> { new Transfer(){Id = 1}}, -1, false},
            new object[] { null, new List<Transfer> { new Transfer(){Id = 1}}, 2, false},
            new object[] { new List<Item> {new(){Uid = "1"}, new(){Uid = "2"}}, new List<Transfer> { new Transfer(){Id = 1, Items = new(){ new(){TransferId = 1, ItemUid = "1"}, new(){TransferId = 1, ItemUid = "2"}}}}, 1, true},
            new object[] { new List<Item> {new(){Uid = "1"}, new(){Uid = "2"}}, new List<Transfer> { new Transfer(){Id = 1}, new Transfer(){Id = 2, Items = new(){ new(){TransferId = 2, ItemUid = "1"}, new(){TransferId = 2, ItemUid = "2"}}}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(RemoveTransferTestData), DynamicDataSourceType.Property)]
    public void TestRemove(List<Item> items, List<Transfer> transfers, int idToRemove, bool expectedResult)
    {
        // Arrange
        addTestResourceToDB(items);

        foreach (Transfer transfer in transfers)
        {
            addTestResourceToDB(transfer.Items);

            db.Transfers.Add(transfer);
            db.SaveChanges();
        }
        TransferDBStorage storage = new(db);

        // Act
        bool actualResult = storage.deleteTransfer(idToRemove).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
        Assert.IsTrue(!db.TransferItems.Select(t => t.TransferId).Contains(idToRemove));
        foreach (TransferItem item in db.TransferItems)
        {
            Assert.IsTrue(!(item.TransferId != idToRemove));
        }
    }

    [TestMethod]
    public void TestRemoveSameTwice()
    {
        // Arrange
        Warehouse t1 = new() { Id = 1 };
        db.Warehouses.Add(t1);
        db.SaveChanges();
        WarehouseDBStorage storage = new(db);


        // Act
        bool firstRemove = storage.deleteWarehouse(t1.Id).Result;
        bool secondRemove = storage.deleteWarehouse(t1.Id).Result;

        // Assert
        Assert.IsTrue(firstRemove == true);
        Assert.IsTrue(secondRemove == false);
    }

    public static IEnumerable<object[]> UpdateTransferTestData => new List<object[]>
        {
            new object[] { null, new List<Transfer> {}, 2, new Transfer(){Id = 1},false},
            new object[] { null, new List<Transfer> {}, 1, null,false},
            new object[] { null, new List<Transfer> {}, 0, new Transfer(){Id = 1},false},
            new object[] { null, new List<Transfer> {}, -1, new Transfer(){Id = 1},false},
            new object[] { new List<Item>{new(){Uid = "1"}, new(){Uid = "2"}}, new List<Transfer> {new Transfer(){Id = 1, Items = new(){ new(){TransferId = 1, ItemUid = "1"}}}}, 1, new Transfer(){Id = 2, Items = { new(){TransferId = 2, ItemUid = "3"}, new(){TransferId = 2, ItemUid = "4"}}}, false},
            new object[] { new List<Item>{new(){Uid = "1"}, new(){Uid = "2"}}, new List<Transfer> {new Transfer(){Id = 1, Items = new(){ new(){TransferId = 1, ItemUid = "1"}}}}, 1, new Transfer(){Id = 2, Items = { new(){TransferId = 3, ItemUid = "1"}, new(){TransferId = 3, ItemUid = "2"}}}, false},
            new object[] { null, new List<Transfer> {new Transfer(){Id = 1}}, 1, new Transfer(){Id = 2}, false},

            new object[] { new List<Item>{new(){Uid = "1"}, new(){Uid = "2"}}, new List<Transfer> {new Transfer(){Id = 1, Items = new(){ new(){TransferId = 1, ItemUid = "1"}}}}, 1, new Transfer(){Id = 1, Items = { new(){TransferId = 2, ItemUid = "1"}, new(){TransferId = 2, ItemUid = "2"}}}, false},
            new object[] { null, new List<Transfer> {new Transfer(){Id = 1}}, 1, new Transfer(){Id = 1}, true},
        };
    [TestMethod]
    [DynamicData(nameof(UpdateTransferTestData), DynamicDataSourceType.Property)]
    public void TestUpdate(List<Item> items, List<Transfer> transfers, int idToUpdate, Transfer updatedTransfer, bool expectedResult)
    {
        // Arrange
        addTestResourceToDB(items);
        addTestResourceToDB(transfers);

        TransferDBStorage storage = new(db);

        // Act
        bool actualResult = storage.updateTransfer(idToUpdate, updatedTransfer).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> CommitTransferTestData => new List<object[]>
    {
        // Expected false because transfer amount of both transferItems is 110 and amount in inventory is 100
        new object[] {
            new List<Inventory> {
                new(){Id = 1, ItemId = "1", InventoryLocations = {new(){InventoryId = 1, LocationId = 1}}, total_on_hand = 100, total_available = 100},
                new(){Id = 2, ItemId = "2", InventoryLocations = {new(){InventoryId = 2, LocationId = 1}}, total_on_hand = 100, total_available = 100}},
            new List<Location> {new(){Id = 1}, new(){Id = 2}},
            new List<Item> {new(){Uid = "1"}, new(){Uid = "2"}},
            new List<Transfer> {new(){
                Id = 1, TransferFrom = 1, TransferTo = 2,
                Items = {
                    new(){ItemUid = "1", TransferId = 1, Amount = 110},
                    new(){ItemUid = "2", TransferId = 1, Amount = 110} }}},
            1,
            false,
            TransferDBStorage.TransferResult.notEnoughItems},
        // Same as above but only one amount is bigger than allowed
        new object[] {
            new List<Inventory> {
                new(){Id = 1, ItemId = "1", InventoryLocations = {new(){InventoryId = 1, LocationId = 1}}, total_on_hand = 100, total_available = 100},
                new(){Id = 2, ItemId = "2", InventoryLocations = {new(){InventoryId = 2, LocationId = 1}}, total_on_hand = 100, total_available = 100}},
            new List<Location> {new(){Id = 1}, new(){Id = 2}},
            new List<Item> {new(){Uid = "1"}, new(){Uid = "2"}},
            new List<Transfer> {new(){
                Id = 1, TransferFrom = 1, TransferTo = 2,
                Items = {
                    new(){ItemUid = "1", TransferId = 1, Amount = 90},
                    new(){ItemUid = "2", TransferId = 1, Amount = 110} }}},
            1,
            false,
            TransferDBStorage.TransferResult.notEnoughItems},
        // Happy Flow test
        new object[] {
            new List<Inventory> {
                new(){Id = 1, ItemId = "1", InventoryLocations = {new(){InventoryId = 1, LocationId = 1}}, total_on_hand = 100, total_available = 100},
                new(){Id = 2, ItemId = "2", InventoryLocations = {new(){InventoryId = 2, LocationId = 1}}, total_on_hand = 100, total_available = 100}},
            new List<Location> {new(){Id = 1}, new(){Id = 2}},
            new List<Item> {new(){Uid = "1"}, new(){Uid = "2"}},
            new List<Transfer> {new(){
                Id = 1, TransferFrom = 1, TransferTo = 2,
                Items = {
                    new(){ItemUid = "1", TransferId = 1, Amount = 90},
                    new(){ItemUid = "2", TransferId = 1, Amount = 90} }}},
            1,
            true,
            TransferDBStorage.TransferResult.possible},
        // Happy Flow test but inventoryLocation should also be emptied from TransferFrom
        new object[] {
            new List<Inventory> {
                new(){Id = 1, ItemId = "1", InventoryLocations = {new(){InventoryId = 1, LocationId = 1}}, total_on_hand = 100, total_available = 100},
                new(){Id = 2, ItemId = "2", InventoryLocations = {new(){InventoryId = 2, LocationId = 1}}, total_on_hand = 100, total_available = 100}},
            new List<Location> {new(){Id = 1}, new(){Id = 2}},
            new List<Item> {new(){Uid = "1"}, new(){Uid = "2"}},
            new List<Transfer> {new(){
                Id = 1, TransferFrom = 1, TransferTo = 2,
                Items = {
                    new(){ItemUid = "1", TransferId = 1, Amount = 100},
                    new(){ItemUid = "2", TransferId = 1, Amount = 100} }}},
            1,
            true,
            TransferDBStorage.TransferResult.possible},
    };
    [TestMethod]
    [DynamicData(nameof(CommitTransferTestData), DynamicDataSourceType.Property)]
    public void TestCommit(List<Inventory> inventories, List<Location> locations, List<Item> items, List<Transfer> transfers, int idToCommit, bool expectedResult, TransferDBStorage.TransferResult expectedMessage)
    {
        // Arrange
        addTestResourceToDB(inventories);
        addTestResourceToDB(items);
        addTestResourceToDB(locations);
        foreach (Transfer transfer in transfers)
        {
            addTestResourceToDB(transfer.Items);

            db.Transfers.Add(transfer);
            db.SaveChanges();
        }

        TransferDBStorage storage = new(db);

        // Act
        List<Inventory> copiedInventories = DeepCopy(inventories);
        (bool succeded, TransferDBStorage.TransferResult message) actualResult = storage.commitTransfer(idToCommit).Result;

        // Assert
        Assert.IsTrue(actualResult.succeded == expectedResult);
        Assert.IsTrue(actualResult.message == expectedMessage);
        // check amount and location of each transferItem
        foreach (Transfer transfer in db.Transfers)
        {
            if (expectedResult == true)
                Assert.IsTrue(transfer.TransferStatus == "Processed");
            foreach (TransferItem transferItem in transfer.Items)
            {
                // false uses the actually original inventories because the original won't be changed if commit fails
                if (expectedResult == false)
                {
                    Assert.IsTrue(db.Inventories.ToList().SequenceEqual(inventories));
                    Assert.IsTrue(db.InventoryLocations.Select(il => il.InventoryId).Contains(transfer.TransferFrom));
                }
                // true uses the copy because the inventories list gets modified in the commit method
                if (expectedResult == true)
                {
                    Assert.IsTrue(!db.Inventories.ToList().SequenceEqual(copiedInventories));
                    Assert.IsTrue(db.InventoryLocations.Select(il => il.InventoryId).Contains(transfer.TransferTo));
                }
            }
        }
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


    public T DeepCopy<T>(T obj, Dictionary<object, object> visitedObjects = null)
    {
        if (obj == null) return default;

        // Initialize the visitedObjects dictionary if not already initialized
        visitedObjects ??= new Dictionary<object, object>();

        // If the object has already been copied, return the copy
        if (visitedObjects.ContainsKey(obj))
            return (T)visitedObjects[obj];

        // If the type is string (or any other type that should not be deep-copied like primitives)
        if (obj is string || obj.GetType().IsPrimitive)
            return obj; // No need to copy, return as is

        // Create a new instance of the object type
        T copy = (T)Activator.CreateInstance(obj.GetType());

        // Add the current object to the visitedObjects dictionary
        visitedObjects[obj] = copy;

        // Get all the fields (public, private, and instance)
        foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var value = field.GetValue(obj);

            // Handle arrays specifically
            if (value is Array arrayValue)
            {
                // Create a new array of the same type and copy the elements
                Array copiedArray = Array.CreateInstance(arrayValue.GetType().GetElementType(), arrayValue.Length);

                // Deep copy each element of the array
                for (int i = 0; i < arrayValue.Length; i++)
                {
                    var element = arrayValue.GetValue(i);
                    if (element != null && element.GetType().IsClass)
                    {
                        // Recursively copy each element in the array, while checking for circular references
                        copiedArray.SetValue(DeepCopy(element, visitedObjects), i);
                    }
                    else
                    {
                        // For primitive types, just copy the value
                        copiedArray.SetValue(element, i);
                    }
                }

                field.SetValue(copy, copiedArray);
            }
            else if (value is ICollection collectionValue)
            {
                // For ICollection types (e.g., List<T>, HashSet<T>, etc.), create a new collection and add the cloned elements
                var copiedCollection = (ICollection)Activator.CreateInstance(value.GetType());
                foreach (var item in collectionValue)
                {
                    var copiedItem = DeepCopy(item, visitedObjects);  // Recursively copy each item
                    copiedCollection.GetType().GetMethod("Add").Invoke(copiedCollection, new[] { copiedItem });
                }
                field.SetValue(copy, copiedCollection);
            }
            else if (value != null && value.GetType().IsClass)
            {
                // For reference types, recursively clone them, while checking for circular references
                field.SetValue(copy, DeepCopy(value, visitedObjects));
            }
            else
            {
                // For primitive types, just copy the value
                field.SetValue(copy, value);
            }
        }

        return copy;
    }
}