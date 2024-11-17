using Microsoft.EntityFrameworkCore;
using CargoHub.Models;
using System.Diagnostics.CodeAnalysis;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class DockDBTest
{
    private DatabaseContext db;

    [TestInitialize]
    public void SetUp()
    {
        // Set up 
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        db = new DatabaseContext(options);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Dispose of the database context after each test
        db?.Dispose();
    }

    public static IEnumerable<object[]> TestGetDocksTestData => new List<object[]>
    {
        new object[] { new List<Dock>() },
        new object[] { new List<Dock> { new Dock { Id = 1, ZipCode = "12345", TransferID = 1, created_at = DateTime.Now, updated_at = DateTime.Now } } },
        new object[] { new List<Dock>
            {
                new Dock { Id = 1, ZipCode = "12345", TransferID = 1, created_at = DateTime.Now, updated_at = DateTime.Now },
                new Dock { Id = 2, ZipCode = "67890", TransferID = 2, created_at = DateTime.Now, updated_at = DateTime.Now }
            }
        }
    };

    [TestMethod]
    [DynamicData(nameof(TestGetDocksTestData), DynamicDataSourceType.Property)]
    public async Task TestGetDocks(List<Dock> docks)
    {
        // Arrange
        await db.Docks.AddRangeAsync(docks);
        await db.SaveChangesAsync();

        DockDBStorage storage = new(db);

        // Act
        IEnumerable<Dock> result = await storage.getDocks();

        // Assert
        Assert.AreEqual(docks.Count, result.Count());
        CollectionAssert.AreEqual(docks, result.ToList());
    }

    public static IEnumerable<object[]> TestGetDockTestData => new List<object[]>
    {
        new object[] { new Dock { Id = 1, ZipCode = "12345", TransferID = 1, created_at = DateTime.Now, updated_at = DateTime.Now }, 1, true },
        new object[] { new Dock { Id = 2, ZipCode = "67890", TransferID = 2, created_at = DateTime.Now, updated_at = DateTime.Now }, 3, false }
    };

    [TestMethod]
    [DynamicData(nameof(TestGetDockTestData), DynamicDataSourceType.Property)]
    public async Task TestGetDock(Dock dock, int id, bool expected)
    {
        // Arrange
        await db.Docks.AddAsync(dock);
        await db.SaveChangesAsync();

        DockDBStorage storage = new(db);

        // Act
        Dock? result = await storage.getDock(id);

        // Assert
        Assert.AreEqual(expected, result != null && result.Equals(dock));
    }

    public static IEnumerable<object[]> TestAddDockTestData => new List<object[]>
    {
        new object[] { new Dock { Id = 1, ZipCode = "12345", TransferID = 1, created_at = DateTime.Now, updated_at = DateTime.Now } },
        new object[] { new Dock { Id = 2, ZipCode = "67890", TransferID = 2, created_at = DateTime.Now, updated_at = DateTime.Now } }
    };

    [TestMethod]
    [DynamicData(nameof(TestAddDockTestData), DynamicDataSourceType.Property)]
    public async Task TestAddDock(Dock dock)
    {
        // Arrange
        DockDBStorage storage = new(db);

        // Act
        bool added = await storage.addDock(dock);

        // Assert
        Assert.IsTrue(added);
        Dock? result = await db.Docks.FirstOrDefaultAsync(x => x.Id == dock.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(dock.ZipCode, result.ZipCode);
    }

    public static IEnumerable<object[]> TestUpdateDockTestData => new List<object[]>
    {
        new object[] { new Dock { Id = 1, ZipCode = "12345", TransferID = 1, created_at = DateTime.Now, updated_at = DateTime.Now }, 1, true },
        new object[] { new Dock { Id = 2, ZipCode = "67890", TransferID = 2, created_at = DateTime.Now, updated_at = DateTime.Now }, 3, false }
    };

    [TestMethod]
    [DynamicData(nameof(TestUpdateDockTestData), DynamicDataSourceType.Property)]
    public async Task TestUpdateDock(Dock dock, int idToUpdate, bool expected)
    {
        // Arrange
        await db.Docks.AddAsync(dock);
        await db.SaveChangesAsync();

        DockDBStorage storage = new(db);

        // Modify the dock for updating
        dock.ZipCode = "UpdatedZipCode";

        // Act
        bool updated = await storage.updateDock(idToUpdate, dock);

        // Assert
        Assert.AreEqual(expected, updated);
        if (expected)
        {
            Dock? updatedDock = await db.Docks.FirstOrDefaultAsync(x => x.Id == idToUpdate);
            Assert.IsNotNull(updatedDock);
            Assert.AreEqual(dock.ZipCode, updatedDock?.ZipCode);
        }
    }

    public static IEnumerable<object[]> TestDeleteDockTestData => new List<object[]>
    {
        new object[] { new Dock { Id = 1, ZipCode = "12345", TransferID = 1, created_at = DateTime.Now, updated_at = DateTime.Now } },
        new object[] { new Dock { Id = 2, ZipCode = "67890", TransferID = 2, created_at = DateTime.Now, updated_at = DateTime.Now } }
    };

    [TestMethod]
    [DynamicData(nameof(TestDeleteDockTestData), DynamicDataSourceType.Property)]
    public async Task TestDeleteDock(Dock dock)
    {
        // Arrange
        await db.Docks.AddAsync(dock);
        await db.SaveChangesAsync();

        DockDBStorage storage = new(db);

        // Act
        bool deleted = await storage.deleteDock(dock.Id);

        // Assert
        Assert.IsTrue(deleted);
        Dock? result = await db.Docks.FirstOrDefaultAsync(x => x.Id == dock.Id);
        Assert.IsNull(result);
    }
}