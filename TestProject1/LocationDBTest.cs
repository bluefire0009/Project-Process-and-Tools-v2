using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class LocationDBTest
{
    private DatabaseContext db;

    [TestInitialize]
    public void setUp()
    {
        // set up mock in memory database at start of each test so the real db doesnt get affected
        var options = new DbContextOptionsBuilder<DatabaseContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
        db = new DatabaseContext(options);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Dispose of the context at the end of each test
        db?.Dispose();
    }

    public static IEnumerable<object[]> TestGetLocationsTestData => new List<object[]>
        {
            new object[] { new List<Location> {}},
            new object[] { new List<Location> { new Location()}},
            new object[] { new List<Location> { new Location(), new Location() }}
        };

    [TestMethod]
    [DynamicData(nameof(TestGetLocationsTestData), DynamicDataSourceType.Property)]
    public async Task TestGetLocations(List<Location> locations)
    {
        // Arrange
        await db.Locations.AddRangeAsync(locations); // Use AddRangeAsync for multiple entries
        await db.SaveChangesAsync();

        LocationStorage storage = new(db);

        // Act
        IEnumerable<Location> x = await storage.GetLocations();
        List<Location> result = x.ToList();

        // Assert
        Assert.IsTrue(result.Count == locations.Count);

        for (int i = 0; i < locations.Count; i++)
        {
            Assert.IsTrue(locations[i].Equals(result[i]));
        }
    }

    public static IEnumerable<object[]> TestGetLocationTestData => new List<object[]>
        {
            new object[] { null, 1, true},
            new object[] { new Location(){Id = 1}, 2, false},
            new object[] { new Location(){Id = 1}, 1, true},
        };

    [TestMethod]
    [DynamicData(nameof(TestGetLocationTestData), DynamicDataSourceType.Property)]
    public async Task TestGetLocation(Location location, int id, bool expected)
    {
        // Arrange
        if (location != null)
        {
            await db.Locations.AddAsync(location);
            await db.SaveChangesAsync();
        }

        LocationStorage storage = new(db);

        // Act
        Location? result = await storage.GetLocation(id);

        // Assert
        Assert.IsTrue((result == location) == expected);
    }

    public static IEnumerable<object[]> TestGetLocationsInWarehousesTestData => new List<object[]>
        {
            new object[] { new List<Location> {}, 1},
            new object[] { new List<Location> { new Location(){Id = 1, WareHouseId = 1}}, 1},
            new object[] { new List<Location> { new Location() { Id = 1, WareHouseId = 1 }, new Location() { Id = 2, WareHouseId = 2 } }, 2 }
        };

    [TestMethod]
    [DynamicData(nameof(TestGetLocationsInWarehousesTestData), DynamicDataSourceType.Property)]
    public async Task TestMethodGetLocationsInWarehouses(List<Location> locations, int GivenWareHouseId)
    {
        // Arrange
        await db.Locations.AddRangeAsync(locations);
        await db.SaveChangesAsync();

        LocationStorage storage = new(db);

        // Act
        List<Location> FoundLocations = (await storage.GetLocationsInWarehouses(GivenWareHouseId)).ToList();

        List<Location> TestLocations = locations.Where(x => x.WareHouseId == GivenWareHouseId).ToList();

        // Assert
        Assert.IsTrue(TestLocations.SequenceEqual(FoundLocations));
    }

    public static IEnumerable<object[]> TestAddLocationTestData => new List<object[]>
        {
            new object[] { new Location(){Id = 1}},
            new object[] { new Location(){Id = 2}},
        };

    [TestMethod]
    [DynamicData(nameof(TestAddLocationTestData), DynamicDataSourceType.Property)]
    public async Task TestAddLocation(Location location)
    {
        // Arrange
        LocationStorage storage = new(db);

        // Act
        await storage.AddLocation(location);

        Location? Result = await db.Locations.FirstOrDefaultAsync(x => x.Id == location.Id);

        // Assert
        Assert.IsTrue(Result == location);
    }

    public static IEnumerable<object[]> TestUpdateLocationTestData => new List<object[]>
        {
            new object[] { new Location(){Id = 1, Code = "abc", Name = "testName", WareHouseId = 12 }, 1, true},
            new object[] { new Location(){Id = 1, Code = "abc", Name = "testName", WareHouseId = 12 }, 2, false},
            new object[] { new Location(){Id = 2, Code = "xyz", Name = "testName2", WareHouseId = 99 } , 2, true},
        };
    [TestMethod]
    [DynamicData(nameof(TestUpdateLocationTestData), DynamicDataSourceType.Property)]
    public async Task TestUpdateLocation(Location location, int Id, bool expected)
    {
        // Arrange
        await db.Locations.AddAsync(location);
        await db.SaveChangesAsync();

        LocationStorage storage = new(db);



        location.Id = 99999999;
        location.Code = "bbb";
        location.Name = "aaa";
        location.WareHouseId = 0;

        // Act
        // Assert
        Assert.IsTrue(await storage.UpdateLocation(Id, location) == expected);
        if (expected)
        {
            Location? UpdatedLocation = db.Locations.FirstOrDefault(x => x.Id == Id);
            Assert.IsTrue(UpdatedLocation == location);
        }
    }

    public static IEnumerable<object[]> DeleteLocationTestData => new List<object[]>
        {
            new object[] { new Location(){Id = 1, Code = "abc", Name = "testName", WareHouseId = 12 }},
            new object[] { new Location(){Id = 2, Code = "xyz", Name = "testName2", WareHouseId = 99 }},
        };

    [TestMethod]
    [DynamicData(nameof(DeleteLocationTestData), DynamicDataSourceType.Property)]
    public async Task TestDeleteLocation(Location location)
    {
        // Arrange
        await db.Locations.AddAsync(location);
        await db.SaveChangesAsync();

        LocationStorage storage = new(db);

        // Act
        // Assert
        Assert.IsTrue(await storage.DeleteLocation(location.Id));

        Assert.IsTrue(await db.Locations.FirstOrDefaultAsync(x => x.Id == location.Id) == null);
    }


    public static IEnumerable<object[]> TestGetLocationsTestDataPagination => new List<object[]>
    {
    new object[] { Enumerable.Range(1, 0).Select(id => new Location { Id = id }).ToList(), 0, 5 },  //   0 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Location { Id = id }).ToList(), 0, 5 }, //   0 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Location { Id = id }).ToList(), 5, 5 }, //   5 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Location { Id = id }).ToList(), 8, 5 }, //   8 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Location { Id = id }).ToList(), 10, 5 }  //  10 offset, limit 5
    };

    [TestMethod]
    [DynamicData(nameof(TestGetLocationsTestDataPagination), DynamicDataSourceType.Property)]
    public async Task TestGetLocationsWithPagination(List<Location> locations, int offset, int limit)
    {
        // Arrange
        await db.Locations.AddRangeAsync(locations); // Add the test data
        await db.SaveChangesAsync();

        LocationStorage storage = new(db);

        // Act
        IEnumerable<Location> x = await storage.GetLocations(offset, limit, true);
        List<Location> result = x.ToList();

        // Console.WriteLine($"offset: {offset}  limit:{limit}  count in db:{locations.Count()}  result count:{result.Count()}");
        // foreach (Location location in result)
        // {
        //     Console.WriteLine("Location: " + location.Id);
        // }

        // Assert
        int expectedCount = Math.Min(limit, Math.Max(0, locations.Count - offset));
        Assert.AreEqual(expectedCount, result.Count, "Returned result count is incorrect.");

        for (int i = 0; i < result.Count; i++)
        {
            Assert.AreEqual(locations[offset + i].Id, result[i].Id, "Location ID does not match at index " + i);
        }
    }

}