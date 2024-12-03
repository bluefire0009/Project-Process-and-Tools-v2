using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;
using CargoHub.HelperFuctions;


namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class ShipmentDBTest
{
    private DatabaseContext db = null!;

    [TestInitialize]
    public void setUp()
    {
        // set up mock in memory database at start of each test so the real db doesnt get affected
        var options = new DbContextOptionsBuilder<DatabaseContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
        db = new DatabaseContext(options);
        PostTestData(db);
    }

    private async void PostTestData(DatabaseContext DB)
    {
        // Fields other than the [Key] can be left empty as they don't matter for the test
        Warehouse warehouse1 = new() { Id = 1 };
        DB.Warehouses.Add(warehouse1);

        Location location1 = new() { Id = 1 };
        DB.Locations.Add(location1);

        // invenories need total fields for testing item counts
        Inventory inventory1 = new()
        {
            Id = 1,
            ItemId = "P000001",
            total_on_hand = 100,
            total_expected = 0,
            total_ordered = 0,
            total_allocated = 0,
            total_available = 100
        };
        DB.Inventories.Add(inventory1);

        Inventory inventory2 = new()
        {
            Id = 2,
            ItemId = "P000002",
            total_on_hand = 50,
            total_expected = 0,
            total_ordered = 0,
            total_allocated = 0,
            total_available = 50
        };
        DB.Inventories.Add(inventory2);

        Inventory inventory3 = new()
        {
            Id = 3,
            ItemId = "P000003",
            total_on_hand = 5,
            total_expected = 0,
            total_ordered = 0,
            total_allocated = 0,
            total_available = 5
        };
        DB.Inventories.Add(inventory3);
        await DB.SaveChangesAsync();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Dispose of the context at the end of each test
        db?.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(TestShipmentData), typeof(ShipmentDBTest))]
    public async Task TestGetShipments(List<Shipment> shipments, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        // Arrange && Act
        ShipmentStorage storage = new(db);

        foreach (var shipment in shipments)
        {
            // add each order and assert that the order has been added
            bool shipmentpostsucces = (await storage.AddShipment(shipment));
            Assert.IsTrue(shipmentpostsucces);
        }
        var FoundShipments = await storage.GetShipments();

        List<Inventory> inventories = GetTestInventories();

        Assert.IsTrue(FoundShipments.Count() == shipments.Count());
        AssertInventoryAmounts(expectedInventories, inventories);

    }

    [TestMethod]
    [DynamicData(nameof(TestShipmentData), typeof(ShipmentDBTest))]
    public async Task TestGetShipment(List<Shipment> shipments, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        // Arrange && Act
        ShipmentStorage storage = new(db);

        foreach (var shipment in shipments)
        {
            // Add each shipment and assert that it has been added
            bool shipmentPostSuccess = await storage.AddShipment(shipment);
            Assert.IsTrue(shipmentPostSuccess);
        }

        foreach (var shipment in shipments)
        {
            Shipment? foundShipment = await storage.GetShipment(shipment.Id);
            Assert.IsNotNull(foundShipment);

            // Use deep equality comparison for complex types like Shipment
            Assert.IsTrue(foundShipment.Equals(shipment));
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestShipmentData), typeof(ShipmentDBTest))]
    public async Task TestGetItemsInShipment(List<Shipment> shipments, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        // Arrange && Act
        ShipmentStorage storage = new(db);

        foreach (var shipment in shipments)
        {
            // Add each shipment and assert that the shipment has been added
            bool shipmentPostSuccess = await storage.AddShipment(shipment);
            Assert.IsTrue(shipmentPostSuccess);
        }

        foreach (var shipment in shipments)
        {
            List<ShipmentItems?> foundShipmentItems = (await storage.GetItemsInShipment(shipment.Id)).ToList()!;
            Assert.IsTrue(foundShipmentItems != null);

            // Use this to ignore the order of the items
            Assert.IsTrue(new HashSet<ShipmentItems>(foundShipmentItems!).SetEquals(shipment.Items));
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestSingleShipmentData), typeof(ShipmentDBTest))]
    public async Task TestUpdateShipment(List<Shipment> shipments, List<List<Tuple<int, int, int, int, int>>> listOfExpectedInventories, List<string> newShipmentStatuses)
    {
        // A shipment can have statuses: {'Pending', 'Transit', 'Delivered'}

        ShipmentStorage storage = new(db);

        foreach (var shipment in shipments)
        {
            // Add each shipment and assert that the shipment has been added
            bool shipmentPostSuccess = await storage.AddShipment(shipment);
            Assert.IsTrue(shipmentPostSuccess);
        }

        for (int i = 0; i < newShipmentStatuses.Count; i++)
        {
            // Change shipment status and assert inventory amounts
            var newShipmentStatus = newShipmentStatuses[i];
            var expectedInventories = listOfExpectedInventories[i];

            // Update the shipment
            Shipment clonedShipment = ObjectCopier.Clone(shipments[0]);
            clonedShipment.ShipmentStatus = newShipmentStatus;

            Assert.IsTrue(await storage.UpdateShipment(shipments[0].Id, clonedShipment));
            // GetTestInventories(newShipmentStatus, shipments[0].ShipmentType!);
            // Compare expected inventories to actual inventories
            AssertInventoryAmounts(expectedInventories, GetTestInventories());
        }
    }


    [TestMethod]
    public async Task TestOrderIds()
    {
        ShipmentStorage storage = new(db);

        Shipment testShipment = new()
        {
            Id = 10,
            OrderIds = new List<OrdersInShipment> { new OrdersInShipment(1, 1), new OrdersInShipment(1, 2), new OrdersInShipment(1, 3) }
        };

        bool postsucces = await storage.AddShipment(testShipment);
        Assert.IsTrue(postsucces);

        Shipment? FoundOrder = await storage.GetShipment(testShipment.Id);
        Assert.IsNotNull(FoundOrder);

        Assert.AreEqual(3, FoundOrder.OrderIds.Count());
    }

    [TestMethod]
    public async Task TestAddingOrderIds()
    {
        ShipmentStorage storage = new(db);

        Shipment testShipment = new()
        {
            Id = 1,
            OrderIds = new List<OrdersInShipment> { new OrdersInShipment(1, 1) }
        };

        Shipment updatedTestShipment = new()
        {
            Id = 1,
            OrderIds = new List<OrdersInShipment> { new OrdersInShipment(1, 1), new OrdersInShipment(2, 1), new OrdersInShipment(3, 1) }
        };

        bool orderpostsucces = await storage.AddShipment(testShipment);
        Assert.IsTrue(orderpostsucces);

        bool orderupdatesucces = await storage.UpdateShipment(updatedTestShipment.Id, updatedTestShipment);
        Assert.IsTrue(orderupdatesucces);

        Shipment? FoundShipment = await storage.GetShipment(updatedTestShipment.Id);
        Assert.IsNotNull(FoundShipment);

        Assert.AreEqual(3, FoundShipment.OrderIds.Count());
    }

    public static IEnumerable<object[]> TestGetShipmentsTestDataPagination => new List<object[]>
    {
    new object[] { Enumerable.Range(1, 0).Select(id => new Shipment { Id = id }).ToList(), 0, 5 },  //   0 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Shipment { Id = id }).ToList(), 0, 5 }, //   0 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Shipment { Id = id }).ToList(), 5, 5 }, //   5 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Shipment { Id = id }).ToList(), 8, 5 }, //   8 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Shipment { Id = id }).ToList(), 10, 5 }  //  10 offset, limit 5
    };

    [TestMethod]
    [DynamicData(nameof(TestGetShipmentsTestDataPagination), DynamicDataSourceType.Property)]
    public async Task TestGetShipmentsWithPagination(List<Shipment> shipments, int offset, int limit)
    {
        // Arrange
        await db.Shipments.AddRangeAsync(shipments); // Add the test data
        await db.SaveChangesAsync();

        ShipmentStorage storage = new(db);

        // Act
        IEnumerable<Shipment> x = await storage.GetShipments(offset, limit, true);
        List<Shipment> result = x.ToList();

        // Console.WriteLine($"offset: {offset}  limit:{limit}  count in db:{shipments.Count()}  result count:{result.Count()}");
        // foreach (Shipment location in result)
        // {
        //     Console.WriteLine("Location: " + location.Id);
        // }


        // Assert
        int expectedCount = Math.Min(limit, Math.Max(0, shipments.Count - offset));
        Assert.AreEqual(expectedCount, result.Count, "Returned result count is incorrect.");

        for (int i = 0; i < result.Count; i++)
        {
            Assert.AreEqual(shipments[offset + i].Id, result[i].Id, "Shipment ID does not match at index " + i);
        }
    }


    // Unique Shipment Types:
    // { 'O', 'I'}

    // Unique Shipment Statuses:
    // { 'Pending', 'Transit', 'Delivered'}
    public static IEnumerable<object[]> TestShipmentData => new List<object[]>
    {
        new object[]
        {
            // First test shipment
            new List<Shipment>
            {
                new Shipment
                {
                    Id = 1,
                    OrderIds =  new List<OrdersInShipment> { new OrdersInShipment(1, 1) },
                    SourceId = 201,
                    OrderDate = DateTime.Parse("2024-11-01 10:00:00"),
                    RequestDate = DateTime.Parse("2024-11-02 12:00:00"),
                    ShipmentDate = DateTime.Parse("2024-11-03 15:00:00"),
                    ShipmentType = "O",  // Type 'O'
                    ShipmentStatus = "Pending",  // Status 'Pending'
                    Notes = "Urgent shipment",
                    CarrierCode = "DHL",
                    CarrierDescription = "DHL Express",
                    ServiceCode = "EXPRESS",
                    PaymentType = "Prepaid",
                    TransferMode = "Air",
                    TotalPackageCount = 5,
                    TotalPackageWeight = 12.5f,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(1),
                    Items = new List<ShipmentItems>
                    {
                        new ShipmentItems("P000001", 20, 1),
                        new ShipmentItems("P000002", 10, 1)
                    }
                }
            },
                    new List<Tuple<int, int, int, int, int>> {
                        new Tuple<int, int, int, int, int>(100, 0, 0, 20, 80),
                        new Tuple<int, int, int, int, int>(50, 0, 0, 10, 40),
                        new Tuple<int, int, int, int, int>(5, 0, 0, 0, 5)
                    }
        },
        new object[]
        {
            // Second test shipment covering ShipmentType 'O' with all ShipmentStatuses
            new List<Shipment>
            {
                new Shipment
                {
                    Id = 2,
                    OrderIds =  new List<OrdersInShipment> { new OrdersInShipment(2, 2) },
                    SourceId = 202,
                    OrderDate = DateTime.Parse("2024-11-04 09:30:00"),
                    RequestDate = DateTime.Parse("2024-11-05 14:00:00"),
                    ShipmentDate = DateTime.Parse("2024-11-06 16:00:00"),
                    ShipmentType = "O",  // Type 'O'
                    ShipmentStatus = "Transit",  // Status 'Transit'
                    Notes = "Standard shipment",
                    CarrierCode = "UPS",
                    CarrierDescription = "UPS Ground",
                    ServiceCode = "GROUND",
                    PaymentType = "Collect",
                    TransferMode = "Ground",
                    TotalPackageCount = 3,
                    TotalPackageWeight = 15.0f,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(2),
                    Items = new List<ShipmentItems>
                    {
                        new ShipmentItems("P000001", 5, 2),
                        new ShipmentItems("P000003", 15, 2)
                    }
                },
                new Shipment
                {
                    Id = 3,
                    OrderIds =  new List<OrdersInShipment> { new OrdersInShipment(3, 3) },
                    SourceId = 203,
                    OrderDate = DateTime.Parse("2024-11-07 13:00:00"),
                    RequestDate = DateTime.Parse("2024-11-08 16:00:00"),
                    ShipmentDate = DateTime.Parse("2024-11-09 18:00:00"),
                    ShipmentType = "O",  // Type 'O'
                    ShipmentStatus = "Delivered",  // Status 'Delivered'
                    Notes = "Express shipment",
                    CarrierCode = "FedEx",
                    CarrierDescription = "FedEx Express",
                    ServiceCode = "EXPR",
                    PaymentType = "Prepaid",
                    TransferMode = "Air",
                    TotalPackageCount = 2,
                    TotalPackageWeight = 8.0f,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(1),
                    Items = new List<ShipmentItems>
                    {
                        new ShipmentItems("P000001", 25, 3)
                    }
                }
            },
                    new List<Tuple<int, int, int, int, int>> {
                        new Tuple<int, int, int, int, int>(70, 0, 0, 0, 70),
                        new Tuple<int, int, int, int, int>(50, 0, 0, 0, 50),
                        new Tuple<int, int, int, int, int>(-10, 0, 0, 0, -10)
            }
        },
        new object[]
        {
            // Third test shipment covering ShipmentType 'I' with all ShipmentStatuses
            new List<Shipment>
            {
                new Shipment
                {
                    Id = 4,
                    OrderIds =  new List<OrdersInShipment> { new OrdersInShipment(4, 4) },
                    SourceId = 204,
                    OrderDate = DateTime.Parse("2024-11-10 11:00:00"),
                    RequestDate = DateTime.Parse("2024-11-11 14:30:00"),
                    ShipmentDate = DateTime.Parse("2024-11-12 17:00:00"),
                    ShipmentType = "I",  // Type 'I'
                    ShipmentStatus = "Pending",  // Status 'Pending'
                    Notes = "Bulk shipment",
                    CarrierCode = "TNT",
                    CarrierDescription = "TNT Freight",
                    ServiceCode = "BULK",
                    PaymentType = "Collect",
                    TransferMode = "Ground",
                    TotalPackageCount = 4,
                    TotalPackageWeight = 20.0f,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(2),
                    Items = new List<ShipmentItems>
                    {
                        new ShipmentItems("P000002", 40, 4),
                        new ShipmentItems("P000003", 60, 4)
                    }
                },
                new Shipment
                {
                    Id = 5,
                    OrderIds =  new List<OrdersInShipment> { new OrdersInShipment(5, 5) },
                    SourceId = 205,
                    OrderDate = DateTime.Parse("2024-11-13 12:00:00"),
                    RequestDate = DateTime.Parse("2024-11-14 15:00:00"),
                    ShipmentDate = DateTime.Parse("2024-11-15 18:00:00"),
                    ShipmentType = "I",  // Type 'I'
                    ShipmentStatus = "Transit",  // Status 'Transit'
                    Notes = "International shipment",
                    CarrierCode = "DHL",
                    CarrierDescription = "DHL Worldwide",
                    ServiceCode = "INTL",
                    PaymentType = "Prepaid",
                    TransferMode = "Air",
                    TotalPackageCount = 3,
                    TotalPackageWeight = 18.5f,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(3),
                    Items = new List<ShipmentItems>
                    {
                        new ShipmentItems("P000001", 30, 5)
                    }
                },
                new Shipment
                {
                    Id = 6,
                    OrderIds =  new List<OrdersInShipment> { new OrdersInShipment(6, 6) },
                    SourceId = 206,
                    OrderDate = DateTime.Parse("2024-11-16 14:00:00"),
                    RequestDate = DateTime.Parse("2024-11-17 16:00:00"),
                    ShipmentDate = DateTime.Parse("2024-11-18 19:00:00"),
                    ShipmentType = "I",  // Type 'I'
                    ShipmentStatus = "Delivered",  // Status 'Delivered'
                    Notes = "Final shipment",
                    CarrierCode = "FedEx",
                    CarrierDescription = "FedEx Freight",
                    ServiceCode = "LTL",
                    PaymentType = "Collect",
                    TransferMode = "Ground",
                    TotalPackageCount = 5,
                    TotalPackageWeight = 25.0f,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(4),
                    Items = new List<ShipmentItems>
                    {
                        new ShipmentItems("P000001", 50, 6),
                        new ShipmentItems("P000002", 70, 6)
                    }
                }
            },
                    new List<Tuple<int, int, int, int, int>> {
                        new Tuple<int, int, int, int, int>(150, 30, 0, 0, 150),
                        new Tuple<int, int, int, int, int>(120, 40, 0, 0, 120),
                        new Tuple<int, int, int, int, int>(5, 60, 0, 0, 5)
                    }
        }
    };

    public static IEnumerable<object[]> TestSingleShipmentData => new List<object[]>
    {
        new object[]
        {
            // First test shipment
            new List<Shipment>
            {
                new Shipment
                {
                    Id = 1,
                    OrderIds =  new List<OrdersInShipment> { new OrdersInShipment(1, 1) },
                    SourceId = 1,
                    OrderDate = DateTime.Parse("2024-11-15T10:00:00Z"),
                    RequestDate = DateTime.Parse("2024-11-16T12:00:00Z"),
                    ShipmentDate = DateTime.Parse("2024-11-17T15:00:00Z"),
                    ShipmentType = "O",
                    ShipmentStatus = "Pending",
                    Notes = "Test outbound shipment.",
                    CarrierCode = "UPS",
                    CarrierDescription = "UPS Standard",
                    ServiceCode = "GROUND",
                    PaymentType = "Prepaid",
                    TransferMode = "Truck",
                    TotalPackageCount = 10,
                    TotalPackageWeight = 25.5f,
                    CreatedAt = DateTime.Parse("2024-11-15T10:00:00Z"),
                    UpdatedAt = DateTime.Parse("2024-11-15T11:00:00Z"),
                    Items = new List<ShipmentItems>
                    {
                        new ShipmentItems("P000001", 10, 1),
                        new ShipmentItems("P000002", 15, 1),
                        new ShipmentItems("P000003", 5, 1)
                    }
                }
            },
        // Expected inventories for this shipment with ShipmentType = "O"
                new List<List<Tuple<int, int, int, int, int>>>
                {
                    // Pending | O
                    new List<Tuple<int, int, int, int, int>>
                    {
                        new Tuple<int, int, int, int, int>(100, 0, 0, 10, 90),
                        new Tuple<int, int, int, int, int>(50, 0, 0, 15, 35),
                        new Tuple<int, int, int, int, int>(5, 0, 0, 5, 0)
                    },
                    // Transit | O
                    new List<Tuple<int, int, int, int, int>>
                    {
                        new Tuple<int, int, int, int, int>(90, 0, 0, 0, 90),
                        new Tuple<int, int, int, int, int>(35, 0, 0, 0, 35),
                        new Tuple<int, int, int, int, int>(0, 0, 0, 0, 0)
                    },
                    // Delivered | O
                    new List<Tuple<int, int, int, int, int>>
                    {
                        new Tuple<int, int, int, int, int>(90, 0, 0, 0, 90),
                        new Tuple<int, int, int, int, int>(35, 0, 0, 0, 35),
                        new Tuple<int, int, int, int, int>(0, 0, 0, 0, 0)
                    }
                },
            // List of statuses to test
            new List<string> { "Pending", "Transit", "Delivered" }
        },
        new object[]
        {
            // Second test shipment with ShipmentType = 'I'
            new List<Shipment>
            {
                new Shipment
                {
                    Id = 2,
                    OrderIds =  new List<OrdersInShipment> { new OrdersInShipment(2, 2) },
                    SourceId = 2,
                    OrderDate = DateTime.Parse("2024-11-18T09:00:00Z"),
                    RequestDate = DateTime.Parse("2024-11-19T14:00:00Z"),
                    ShipmentDate = DateTime.Parse("2024-11-20T17:00:00Z"),
                    ShipmentType = "I",
                    ShipmentStatus = "Pending",
                    Notes = "Test inbound shipment.",
                    CarrierCode = "FedEx",
                    CarrierDescription = "FedEx Priority",
                    ServiceCode = "AIR",
                    PaymentType = "COD",
                    TransferMode = "Air",
                    TotalPackageCount = 5,
                    TotalPackageWeight = 10.0f,
                    CreatedAt = DateTime.Parse("2024-11-18T09:00:00Z"),
                    UpdatedAt = DateTime.Parse("2024-11-18T10:00:00Z"),
                    Items = new List<ShipmentItems>
                    {
                        new ShipmentItems("P000001", 8, 2),
                        new ShipmentItems("P000002", 8, 2),
                        new ShipmentItems("P000003", 12, 2)
                    }
                }
            },
        // Expected inventories for this shipment with ShipmentType = "I"
        new List<List<Tuple<int, int, int, int, int>>>
        {
            // Pending | I
            new List<Tuple<int, int, int, int, int>>
            {
                new Tuple<int, int, int, int, int>(100, 8, 0, 0, 100),
                new Tuple<int, int, int, int, int>(50, 8, 0, 0, 50),
                new Tuple<int, int, int, int, int>(5, 12, 0, 0, 5)
            },
            // Transit | I
            new List<Tuple<int, int, int, int, int>>
            {
                new Tuple<int, int, int, int, int>(100, 8, 0, 0, 100),
                new Tuple<int, int, int, int, int>(50, 8, 0, 0, 50),
                new Tuple<int, int, int, int, int>(5, 12, 0, 0, 5)
            },
            // Delivered | I
            new List<Tuple<int, int, int, int, int>>
            {
                new Tuple<int, int, int, int, int>(108, 0, 0, 0, 108),
                new Tuple<int, int, int, int, int>(58, 0, 0, 0, 58),
                new Tuple<int, int, int, int, int>(17, 0, 0, 0, 17)
            }
        },
            // List of statuses to test
            new List<string> { "Pending", "Transit", "Delivered" }
        }
    };


    private List<Inventory> GetTestInventories(string status = "", string type = "")
    {
        // Get all the test inventories
        Inventory? inventory1 = db.Inventories.FirstOrDefault(x => x.Id == 1);
        Inventory? inventory2 = db.Inventories.FirstOrDefault(x => x.Id == 2);
        Inventory? inventory3 = db.Inventories.FirstOrDefault(x => x.Id == 3);
        Assert.IsNotNull(inventory1);
        Assert.IsNotNull(inventory2);
        Assert.IsNotNull(inventory3);

        Console.WriteLine($"--------------- Inventories for test {status} | {type} ------------------");
        Console.WriteLine($"Total On Hand: {inventory1.total_on_hand}, Total Expected: {inventory1.total_expected}, Total Ordered: {inventory1.total_ordered}, Total Allocated: {inventory1.total_allocated}, Total Available: {inventory1.total_available}");
        Console.WriteLine($"Total On Hand: {inventory2.total_on_hand}, Total Expected: {inventory2.total_expected}, Total Ordered: {inventory2.total_ordered}, Total Allocated: {inventory2.total_allocated}, Total Available: {inventory2.total_available}");
        Console.WriteLine($"Total On Hand: {inventory3.total_on_hand}, Total Expected: {inventory3.total_expected}, Total Ordered: {inventory3.total_ordered}, Total Allocated: {inventory3.total_allocated}, Total Available: {inventory3.total_available}");

        List<Inventory> inventories = new() { inventory1, inventory2, inventory3 };
        return inventories;
    }

    private void AssertInventoryAmounts(List<Tuple<int, int, int, int, int>> expectedInventories, List<Inventory> inventories)
    {
        for (int i = 0; i < expectedInventories.Count; i++)
        {
            Assert.IsTrue(expectedInventories[i].Item1 == inventories[i].total_on_hand);
            Assert.IsTrue(expectedInventories[i].Item2 == inventories[i].total_expected);
            Assert.IsTrue(expectedInventories[i].Item3 == inventories[i].total_ordered);
            Assert.IsTrue(expectedInventories[i].Item4 == inventories[i].total_allocated);
            Assert.IsTrue(expectedInventories[i].Item5 == inventories[i].total_available);
        }
    }
}