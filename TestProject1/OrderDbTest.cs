using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class OrderDBTest
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

    private void PostTestData(DatabaseContext DB)
    {
        // Fields other than the [Key] can be left empty as they don't matter for the test
        Warehouse warehouse1 = new() { Id = 1 };
        DB.Warehouses.Add(warehouse1);

        Location location1 = new() { Id = 1 };
        DB.Locations.Add(location1);

        Client client1 = new() { Id = 1 };
        DB.Clients.Add(client1);

        Shipment shipment1 = new() { Id = 1 };
        DB.Shipments.Add(shipment1);

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
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Dispose of the context at the end of each test
        db?.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(TestOrdersData), typeof(OrderDBTest))]
    public async Task TestGetOrders(List<Order> orders, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        // Arrange && Act
        OrderStroage storage = new(db);

        foreach (var order in orders)
        {
            // add each order and assert that the order has been added
            bool orderpostsucces = (await storage.AddOrder(order));
            Assert.IsTrue(orderpostsucces);
        }

        Inventory? inventory1 = db.Inventories.FirstOrDefault(x => x.Id == 1);
        Inventory? inventory2 = db.Inventories.FirstOrDefault(x => x.Id == 2);
        Inventory? inventory3 = db.Inventories.FirstOrDefault(x => x.Id == 3);
        Assert.IsNotNull(inventory1);
        Assert.IsNotNull(inventory2);
        Assert.IsNotNull(inventory3);

        Console.WriteLine($"--------------- Inventories for test ------------------");
        Console.WriteLine($"Total On Hand: {inventory1.total_on_hand}, Total Expected: {inventory1.total_expected}, Total Ordered: {inventory1.total_ordered}, Total Allocated: {inventory1.total_allocated}, Total Available: {inventory1.total_available}");
        Console.WriteLine($"Total On Hand: {inventory2.total_on_hand}, Total Expected: {inventory2.total_expected}, Total Ordered: {inventory2.total_ordered}, Total Allocated: {inventory2.total_allocated}, Total Available: {inventory2.total_available}");
        Console.WriteLine($"Total On Hand: {inventory3.total_on_hand}, Total Expected: {inventory3.total_expected}, Total Ordered: {inventory3.total_ordered}, Total Allocated: {inventory3.total_allocated}, Total Available: {inventory3.total_available}");

        // ---------------Inventories for test------------------
        // Total On Hand: 80, Total Expected: 0, Total Ordered: 0, Total Allocated: 0, Total Available: 80
        // Total On Hand: 50, Total Expected: 0, Total Ordered: 0, Total Allocated: 0, Total Available: 50
        // Total On Hand: 5, Total Expected: 0, Total Ordered: 0, Total Allocated: 0, Total Available: 5
        // -------------- - Inventories for test------------------
        // Total On Hand: 100, Total Expected: 0, Total Ordered: 0, Total Allocated: 0, Total Available: 100
        // Total On Hand: 40, Total Expected: 0, Total Ordered: 0, Total Allocated: 0, Total Available: 40
        // Total On Hand: 0, Total Expected: 0, Total Ordered: 0, Total Allocated: 0, Total Available: 0
        // -------------- - Inventories for test------------------
        // Total On Hand: 100, Total Expected: 0, Total Ordered: 0, Total Allocated: 8, Total Available: 92
        // Total On Hand: 50, Total Expected: 0, Total Ordered: 15, Total Allocated: 2, Total Available: 33
        // Total On Hand: 5, Total Expected: 0, Total Ordered: 20, Total Allocated: 0, Total Available: -15



        // Assert
        // Assert.IsTrue(result.Count() == orders.Count());

        // for (int i = 0; i < orders.Count; i++)
        // {
        //     Assert.IsTrue(orders[i].Equals(result[i]));
        // }
    }


    public static IEnumerable<object[]> TestOrdersData => new List<object[]>
{
    new object[]
    {
        // First test order
        new List<Order>
        {
            new Order
            {
                Id = 1,
                SourceId = 1,
                OrderDate = DateTime.Parse("1971-11-25T18:25:07Z"),
                RequestDate = DateTime.Parse("1971-11-29T18:25:07Z"),
                Reference = "ORD00004",
                OrderStatus = "Delivered",
                Notes = "Licht gebruikelijk melk brug.",
                ShippingNotes = "Afmaken thee fris.",
                PickingNotes = "Meisje vis volgende overal hallo vrijheid gebeurtenis hut.",
                WareHouseId = 1,
                ShipTo = 1,
                BillTo = 1,
                ShipmentId = 1,
                TotalAmount = 6182.77f,
                TotalDiscount = 401.42f,
                TotalTax = 780.29f,
                TotalSurcharge = 85.5f,
                CreatedAt = DateTime.Parse("1971-11-25T18:25:07Z"),
                UpdatedAt = DateTime.Parse("1971-11-27T14:25:07Z"),
                Items = new List<OrderItems>
                {
                    new OrderItems("P000001", 20)
                }
            }
        },
             // Expected inventory values as a tuple
            new List<Tuple<int, int, int, int, int>> {
            new Tuple<int, int, int, int, int>(80, 0, 0, 0, 80), // (Total On Hand, Total Expected, Total Ordered, Total Allocated, Total Available)
            new Tuple<int, int, int, int, int>(50, 0, 0, 0, 50), // unchanged
            new Tuple<int, int, int, int, int>(5, 0, 0, 0, 5), // unchanged
            }
    },
    new object[]
    {
        // Second test order
        new List<Order>
        {
            new Order
            {
                Id = 2,
                SourceId = 2,
                OrderDate = DateTime.Parse("1980-05-15T10:15:30Z"),
                RequestDate = DateTime.Parse("1980-05-20T10:15:30Z"),
                Reference = "ORD00005",
                OrderStatus = "Shipped",
                Notes = "Nieuwe bestelling verwerking.",
                ShippingNotes = "Levering gepland.",
                PickingNotes = "Controleer voorraad en planning.",
                WareHouseId = 1,
                ShipTo = 1,
                BillTo = 1,
                ShipmentId = 1,
                TotalAmount = 1520.50f,
                TotalDiscount = 100.00f,
                TotalTax = 120.75f,
                TotalSurcharge = 50.0f,
                CreatedAt = DateTime.Parse("1980-05-15T10:15:30Z"),
                UpdatedAt = DateTime.Parse("1980-05-16T11:00:00Z"),
                Items = new List<OrderItems>
                {
                    new OrderItems("P000002", 10),
                    new OrderItems("P000003", 5)
                }
            }
        },
        // Expected inventory values for the second set of inventories
        new List<Tuple<int, int, int, int, int>> {
            new Tuple<int, int, int, int, int>(100, 0, 0, 0, 100), // unchanged
            new Tuple<int, int, int, int, int>(40, 0, 0, 0, 40),
            new Tuple<int, int, int, int, int>(0, 0, 0, 0, 0)
        }

    },
    new object[]
    {
        // Multiple test orders
        new List<Order>
        {
            new Order
            {
                Id = 3,
                SourceId = 3,
                OrderDate = DateTime.Parse("1990-12-01T08:00:00Z"),
                RequestDate = DateTime.Parse("1990-12-05T08:00:00Z"),
                Reference = "ORD00006",
                OrderStatus = "Pending",
                Notes = "Wachtend op goedkeuring.",
                ShippingNotes = "Nog niet verzonden.",
                PickingNotes = "Wacht op toewijzing.",
                WareHouseId = 1,
                ShipTo = 1,
                BillTo = 1,
                ShipmentId = 1,
                TotalAmount = 850.25f,
                TotalDiscount = 50.00f,
                TotalTax = 68.75f,
                TotalSurcharge = 25.0f,
                CreatedAt = DateTime.Parse("1990-12-01T08:00:00Z"),
                UpdatedAt = DateTime.Parse("1990-12-03T10:30:00Z"),
                Items = new List<OrderItems>
                {
                    new OrderItems("P000001", 8),
                    new OrderItems("P000002", 2)
                }
            },
            new Order
            {
                Id = 4,
                SourceId = 4,
                OrderDate = DateTime.Parse("2000-01-10T12:30:00Z"),
                RequestDate = DateTime.Parse("2000-01-15T12:30:00Z"),
                Reference = "ORD00007",
                OrderStatus = "Packed",
                Notes = "Bestelling onderweg.",
                ShippingNotes = "Verzonden met tracking.",
                PickingNotes = "Verzonden vanuit magazijn.",
                WareHouseId = 1,
                ShipTo = 1,
                BillTo = 1,
                ShipmentId = 1,
                TotalAmount = 3200.00f,
                TotalDiscount = 300.00f,
                TotalTax = 200.00f,
                TotalSurcharge = 75.0f,
                CreatedAt = DateTime.Parse("2000-01-10T12:30:00Z"),
                UpdatedAt = DateTime.Parse("2000-01-13T09:00:00Z"),
                Items = new List<OrderItems>
                {
                    new OrderItems("P000002", 15),
                    new OrderItems("P000003", 20)
                }
            }
        },
            // Expected inventory values for the third set of inventories
            new List<Tuple<int, int, int, int, int>> {
            new Tuple<int, int, int, int, int>(100, 0, 0, 8, 92),
            new Tuple<int, int, int, int, int>(50, 0, 15, 2, 33),
            new Tuple<int, int, int, int, int>(5, 0, 20, 0, -15)
            }
    }
};

}
