using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class SupplierDBTest
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

    public static IEnumerable<object[]> SuppliersTestData => new List<object[]>
        {
            new object[] { new List<Supplier> {}},
            new object[] { new List<Supplier> { new Supplier()}},
            new object[] { new List<Supplier> { new Supplier(), new Supplier() }}
        };
    [TestMethod]
    [DynamicData(nameof(SuppliersTestData), DynamicDataSourceType.Property)]
    public void TestGetAll(List<Supplier> suppliers)
    {
        // Arrange
        foreach (Supplier supplier in suppliers)
        {
            db.Suppliers.Add(supplier);
            db.SaveChanges();
        }
        SupplierDBStorage storage = new(db);

        // Act
        List<Supplier> result = storage.getSuppliers().Result.ToList();

        // Assert
        Assert.IsTrue(result.Count == suppliers.Count);
        for (int supplierIterator = 0; supplierIterator < result.Count; supplierIterator++)
        {
            Assert.IsTrue(result[supplierIterator].Id == suppliers[supplierIterator].Id);
            Assert.IsTrue(result[supplierIterator].Code == suppliers[supplierIterator].Code);
            Assert.IsTrue(result[supplierIterator].Name == suppliers[supplierIterator].Name);
            Assert.IsTrue(result[supplierIterator].Address == suppliers[supplierIterator].Address);
            Assert.IsTrue(result[supplierIterator].AddressExtra == suppliers[supplierIterator].AddressExtra);
            Assert.IsTrue(result[supplierIterator].City == suppliers[supplierIterator].City);
            Assert.IsTrue(result[supplierIterator].ZipCode == suppliers[supplierIterator].ZipCode);
            Assert.IsTrue(result[supplierIterator].Province == suppliers[supplierIterator].Province);
            Assert.IsTrue(result[supplierIterator].Country == suppliers[supplierIterator].Country);
            Assert.IsTrue(result[supplierIterator].ContactName == suppliers[supplierIterator].ContactName);
            Assert.IsTrue(result[supplierIterator].PhoneNumber == suppliers[supplierIterator].PhoneNumber);
            Assert.IsTrue(result[supplierIterator].Reference == suppliers[supplierIterator].Reference);
            Assert.IsTrue(result[supplierIterator].CreatedAt == suppliers[supplierIterator].CreatedAt);
            Assert.IsTrue(result[supplierIterator].UpdatedAt == suppliers[supplierIterator].UpdatedAt);
        }
    }

    public static IEnumerable<object[]> SpecificSuppliersTestData => new List<object[]>
        {
            new object[] { new List<Supplier> {}, 1, false},
            new object[] { new List<Supplier> { new Supplier(){Id = 1}}, 2, false},
            new object[] { new List<Supplier> { new Supplier(){Id = 1}}, 1, true},
            new object[] { new List<Supplier> { new Supplier(){Id = 1}, new Supplier(){Id = 2}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(SpecificSuppliersTestData), DynamicDataSourceType.Property)]
    public void TestGetSpecific(List<Supplier> suppliers, int soughtId, bool expectedResult)
    {
        // Arrange
        foreach (Supplier supplier in suppliers)
        {
            db.Suppliers.Add(supplier);
            db.SaveChanges();
        }
        SupplierDBStorage storage = new(db);

        // Act
        Supplier? foundSupplier = storage.getSupplier(soughtId).Result;

        // Assert
        bool actualResult = foundSupplier != null;
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> AddSupplierTestData => new List<object[]>
        {
            new object[] { null, false},
            new object[] { new Supplier(){Id = -1}, false},
            new object[] { new Supplier(){Id = 0}, false},
            new object[] { new Supplier(){Id = 1}, true}
        };
    [TestMethod]
    [DynamicData(nameof(AddSupplierTestData), DynamicDataSourceType.Property)]
    public void TestAdd(Supplier supplier, bool expectedResult)
    {
        // Arrange
        SupplierDBStorage storage = new(db);

        // Act
        bool actualResult = storage.addSupplier(supplier).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    [TestMethod]
    public void TestAddSameIdTwice()
    {
        // Arrange
        Supplier s1 = new() { Id = 1 };
        Supplier s2 = new() { Id = 1 };
        SupplierDBStorage storage = new(db);

        // Act
        bool firstAdd = storage.addSupplier(s1).Result;
        bool secondAdd = storage.addSupplier(s2).Result;

        // Assert
        Assert.IsTrue(firstAdd == true);
        Assert.IsTrue(secondAdd == false);
    }

    public static IEnumerable<object[]> RemoveSupplierTestData => new List<object[]>
        {
            new object[] { new List<Supplier> {}, 1, false},
            new object[] { new List<Supplier> { new Supplier(){Id = 1}}, 0, false},
            new object[] { new List<Supplier> { new Supplier(){Id = 1}}, -1, false},
            new object[] { new List<Supplier> { new Supplier(){Id = 1}}, 2, false},
            new object[] { new List<Supplier> { new Supplier(){Id = 1}}, 1, true},
            new object[] { new List<Supplier> { new Supplier(){Id = 1}, new Supplier(){Id = 2}}, 2, true}
        };
    [TestMethod]
    [DynamicData(nameof(RemoveSupplierTestData), DynamicDataSourceType.Property)]
    public void TestRemove(List<Supplier> suppliers, int idToRemove, bool expectedResult)
    {
        // Arrange
        foreach (Supplier supplier in suppliers)
        {
            db.Suppliers.Add(supplier);
            db.SaveChanges();
        }
        SupplierDBStorage storage = new(db);

        // Act
        bool actualResult = storage.deleteSupplier(idToRemove).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    [TestMethod]
    public void TestRemoveSameTwice()
    {
        // Arrange
        Supplier s1 = new() { Id = 1 };
        db.Suppliers.Add(s1);
        db.SaveChanges();
        SupplierDBStorage storage = new(db);


        // Act
        bool firstRemove = storage.deleteSupplier(s1.Id).Result;
        bool secondRemove = storage.deleteSupplier(s1.Id).Result;

        // Assert
        Assert.IsTrue(firstRemove == true);
        Assert.IsTrue(secondRemove == false);
    }

    public static IEnumerable<object[]> UpdateSupplierTestData => new List<object[]>
        {
            new object[] { new List<Supplier> {}, 2, new Supplier(){Id = 1},false},
            new object[] { new List<Supplier> {}, 1, null,false},
            new object[] { new List<Supplier> {}, 0, new Supplier(){Id = 1},false},
            new object[] { new List<Supplier> {}, -1, new Supplier(){Id = 1},false},
            new object[] { new List<Supplier> {new Supplier(){Id = 1}}, 1, new Supplier(){Id = 2}, false},
            new object[] { new List<Supplier> {new Supplier(){Id = 1}}, 1, new Supplier(){Id = 1, Code = "ABC"}, true},
        };
    [TestMethod]
    [DynamicData(nameof(UpdateSupplierTestData), DynamicDataSourceType.Property)]
    public void TestUpdate(List<Supplier> suppliers, int idToUpdate, Supplier updatedSupplier, bool expectedResult)
    {
        // Arrange
        foreach (Supplier supplier in suppliers)
        {
            db.Suppliers.Add(supplier);
            db.SaveChanges();
        }
        SupplierDBStorage storage = new(db);

        // Act
        bool actualResult = storage.updateSupplier(idToUpdate, updatedSupplier).Result;

        // Assert
        Assert.IsTrue(actualResult == expectedResult);
    }

    public static IEnumerable<object[]> GetSupplierItemsTestData => new List<object[]>
        {
            new object[] { new List<Supplier> {new(){Id = 1}}, new List<Item> {new(){Uid = "P000001", SupplierId = 1}, new(){Uid = "P000002", SupplierId = 1}, new(){Uid = "P000003", SupplierId = 1}}, 1},
            new object[] { new List<Supplier> {new(){Id = 1}}, null, 0},
            new object[] { new List<Supplier> {new(){Id = 1}}, null, -1},
            new object[] { new List<Supplier> {new(){Id = 2}}, new List<Item> {new(){Uid = "P000001", SupplierId = 1}, new(){Uid = "P000002", SupplierId = 1}, new(){Uid = "P000003", SupplierId = 1}}, 2},
        };
    [TestMethod]
    [DynamicData(nameof(GetSupplierItemsTestData), DynamicDataSourceType.Property)]
    public void TestGetItems(List<Supplier> suppliers, List<Item> items, int soughtId)
    {
        // Arrange
        foreach (Supplier supplier in suppliers)
        {
            db.Suppliers.Add(supplier);
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
        SupplierDBStorage storage = new(db);

        // Act
        IEnumerable<Item>? result = storage.getSupplierItems(soughtId);
        List<Item> resultList;
        // these two statements are here because getSupplierItems COULD TECHNICALLY return a null, not very likely but still
        if (result == null) resultList = new();
        else resultList = result.ToList();


        // Assert
        for (int itemsIterator = 0; itemsIterator < resultList.Count; itemsIterator++)
        {
            Assert.IsTrue(resultList[itemsIterator].Uid == items[itemsIterator].Uid);
            Assert.IsTrue(resultList[itemsIterator].Code == items[itemsIterator].Code);
            Assert.IsTrue(resultList[itemsIterator].Description == items[itemsIterator].Description);
            Assert.IsTrue(resultList[itemsIterator].ShortDescription == items[itemsIterator].ShortDescription);
            Assert.IsTrue(resultList[itemsIterator].UpcCode == items[itemsIterator].UpcCode);
            Assert.IsTrue(resultList[itemsIterator].ModelNumber == items[itemsIterator].ModelNumber);
            Assert.IsTrue(resultList[itemsIterator].CommodityCode == items[itemsIterator].CommodityCode);
            Assert.IsTrue(resultList[itemsIterator].itemLine == items[itemsIterator].itemLine);
            Assert.IsTrue(resultList[itemsIterator].itemGroup == items[itemsIterator].itemGroup);
            Assert.IsTrue(resultList[itemsIterator].itemType == items[itemsIterator].itemType);
            Assert.IsTrue(resultList[itemsIterator].UnitPurchaseQuantity == items[itemsIterator].UnitPurchaseQuantity);
            Assert.IsTrue(resultList[itemsIterator].UnitOrderQuantity == items[itemsIterator].UnitOrderQuantity);
            Assert.IsTrue(resultList[itemsIterator].PackOrderQuantity == items[itemsIterator].PackOrderQuantity);
            Assert.IsTrue(resultList[itemsIterator].SupplierId == items[itemsIterator].SupplierId);
            Assert.IsTrue(resultList[itemsIterator].SupplierCode == items[itemsIterator].SupplierCode);
            Assert.IsTrue(resultList[itemsIterator].SupplierPartNumber == items[itemsIterator].SupplierPartNumber);
            Assert.IsTrue(resultList[itemsIterator].UpdatedAt == items[itemsIterator].UpdatedAt);
        }
        if (resultList.Count == 0 && result != null)
            Assert.IsTrue(items != null);
        if (result == null)
            Assert.IsTrue(items == null);
    }
}