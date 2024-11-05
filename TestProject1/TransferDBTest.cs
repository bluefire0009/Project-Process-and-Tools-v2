using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

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
        foreach (Transfer transfer in transfers)
        {
            db.Transfers.Add(transfer);
            db.SaveChanges();
        }
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
        foreach (Transfer transfer in transfers)
        {
            db.Transfers.Add(transfer);
            db.SaveChanges();
        }
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
            new object[] { null, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = 1}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = 1}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = 1}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = 1}}}, false},
            new object[] { null, null, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = 0}}}, false},
            new object[] { null, null, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = 0}}}, false},
            new object[] { null, null, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = 0}}}, false},
            new object[] { null, null, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = 0}}}, false},
            new object[] { null, null, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = -1}}}, false},
            new object[] { null, null, new Transfer(){Id = -1, Items = {new() {TransferId = -1, ItemUid = -1}}}, false},
            new object[] { null, null, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = -1}}}, false},
            new object[] { null, null, new Transfer(){Id = 0, Items = {new() {TransferId = 0, ItemUid = -1}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, Items = {new() {TransferId = 1, ItemUid = 1}, new(){TransferId = 1, ItemUid = 1}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, Items = {new() {TransferId = 1, ItemUid = 1}, new(){TransferId = 1, ItemUid = 1}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},
            new object[] { null, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, TransferFrom = 3, TransferTo = 4, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, TransferFrom = 3, TransferTo = 4, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, TransferFrom = 4, TransferTo = 2, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, TransferFrom = 4, TransferTo = 2, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, TransferFrom = 2, TransferTo = 4, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, TransferFrom = 2, TransferTo = 2, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, TransferFrom = 2, TransferTo = 1, Items = {new(){TransferId = 2, ItemUid = 1}}}, false},

            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}}, new Transfer(){Id = 1, TransferFrom = 1, TransferTo = 2, Items = {new(){TransferId = 1, ItemUid = 1}}}, true},
            new object[] { new List<Location>(){new(){Id = 1}, new(){Id = 2}}, new List<Item>(){new(){Uid = 1}, new(){Uid = 2}}, new Transfer(){Id = 1, TransferFrom = 1, TransferTo = 2, Items = {new(){TransferId = 1, ItemUid = 1}, new(){TransferId = 1, ItemUid = 2}}}, true}
        };
    [TestMethod]
    [DynamicData(nameof(AddTransferTestData), DynamicDataSourceType.Property)]
    public void TestAdd(List<Location> locations, List<Item> items, Transfer transfer, bool expectedResult)
    {
        // Arrange
        if (locations != null)
        {
            foreach (Location location in locations)
            {
                db.Locations.Add(location);
                db.SaveChanges();
            }
        }
        if (items != null)
        {
            foreach (Item item in items)
            {
                db.Items.Add(item);
                db.SaveChanges();
            }
        }
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
            new object[] { new List<Item> {new(){Uid = 1}, new(){Uid = 2}}, new List<Transfer> { new Transfer(){Id = 1, Items = new(){ new(){TransferId = 1, ItemUid = 1}, new(){TransferId = 1, ItemUid = 2}}}}, 1, true},
            new object[] { new List<Item> {new(){Uid = 1}, new(){Uid = 2}}, new List<Transfer> { new Transfer(){Id = 1}, new Transfer(){Id = 2, Items = new(){ new(){TransferId = 2, ItemUid = 1}, new(){TransferId = 2, ItemUid = 2}}}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(RemoveTransferTestData), DynamicDataSourceType.Property)]
    public void TestRemove(List<Item> items, List<Transfer> transfers, int idToRemove, bool expectedResult)
    {
        // Arrange
        if (items != null)
        {
            foreach (Item item in items)
            {
                db.Items.Add(item);
                db.SaveChanges();
            }
        }
        foreach (Transfer transfer in transfers)
        {
            foreach (TransferItem item in transfer.Items)
            {
                db.TransferItems.Add(item);
                db.SaveChanges();
            }
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
            new object[] { new List<Item>{new(){Uid = 1}, new(){Uid = 2}}, new List<Transfer> {new Transfer(){Id = 1, Items = new(){ new(){TransferId = 1, ItemUid = 1}}}}, 1, new Transfer(){Id = 2, Items = { new(){TransferId = 2, ItemUid = 3}, new(){TransferId = 2, ItemUid = 4}}}, false},
            new object[] { new List<Item>{new(){Uid = 1}, new(){Uid = 2}}, new List<Transfer> {new Transfer(){Id = 1, Items = new(){ new(){TransferId = 1, ItemUid = 1}}}}, 1, new Transfer(){Id = 2, Items = { new(){TransferId = 3, ItemUid = 1}, new(){TransferId = 3, ItemUid = 2}}}, false},
            new object[] { null, new List<Transfer> {new Transfer(){Id = 1}}, 1, new Transfer(){Id = 2}, false},

            new object[] { new List<Item>{new(){Uid = 1}, new(){Uid = 2}}, new List<Transfer> {new Transfer(){Id = 1, Items = new(){ new(){TransferId = 1, ItemUid = 1}}}}, 1, new Transfer(){Id = 1, Items = { new(){TransferId = 2, ItemUid = 1}, new(){TransferId = 2, ItemUid = 2}}}, false},
            new object[] { null, new List<Transfer> {new Transfer(){Id = 1}}, 1, new Transfer(){Id = 1}, true},
        };
    [TestMethod]
    [DynamicData(nameof(UpdateTransferTestData), DynamicDataSourceType.Property)]
    public void TestUpdate(List<Item> items, List<Transfer> transfers, int idToUpdate, Transfer updatedTransfer, bool expectedResult)
    {
        // Arrange
        if (items != null)
        {
            foreach (Item item in items)
            {
                db.Items.Add(item);
                db.SaveChanges();
            }
        }
        foreach (Transfer transfer in transfers)
        {
            db.Transfers.Add(transfer);
            db.SaveChanges();
        }
        TransferDBStorage storage = new(db);

        // Act
        bool actualResult = storage.updateTransfer(idToUpdate, updatedTransfer).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }
}