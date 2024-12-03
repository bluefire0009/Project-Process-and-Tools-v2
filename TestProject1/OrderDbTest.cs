using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;
using CargoHub.HelperFuctions;


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

    private async void PostTestData(DatabaseContext DB)
    {
        // Fields other than the [Key] can be left empty as they don't matter for the test
        Warehouse warehouse1 = new() { Id = 1 };
        DB.Warehouses.Add(warehouse1);

        Location location1 = new() { Id = 1 };
        DB.Locations.Add(location1);

        Client client1 = new() { Id = 1 };
        DB.Clients.Add(client1);

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
    [DynamicData(nameof(TestOrdersData), typeof(OrderDBTest))]
    public async Task TestGetOrders(List<Order> orders, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        // Arrange && Act
        OrderStorage storage = new(db);

        foreach (var order in orders)
        {
            // add each order and assert that the order has been added
            bool orderpostsucces = (await storage.AddOrder(order));
            Assert.IsTrue(orderpostsucces);
        }

        var FoundOrders = await storage.GetOrders();
        Assert.IsTrue(FoundOrders.Count() == orders.Count());

        List<Inventory> inventories = GetTestInventories();

        // Assert the amounts in the inventories
        AssertInventoryAmounts(expectedInventories, inventories);
    }

    [TestMethod]
    [DynamicData(nameof(TestOrdersData), typeof(OrderDBTest))]
    public async Task TestGetOrder(List<Order> orders, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        // Arrange && Act
        OrderStorage storage = new(db);

        foreach (var order in orders)
        {
            // add each order and assert that the order has been added
            bool orderpostsucces = (await storage.AddOrder(order));
            Assert.IsTrue(orderpostsucces);
        }

        foreach (var order in orders)
        {
            Order? FoundOrder = await storage.GetOrder(order.Id);
            Assert.IsTrue(FoundOrder != null);

            Assert.IsTrue(FoundOrder == order);
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestOrdersData), typeof(OrderDBTest))]
    public async Task TestGetItemsInOrder(List<Order> orders, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        // Arrange && Act
        OrderStorage storage = new(db);

        foreach (var order in orders)
        {
            // add each order and assert that the order has been added
            bool orderpostsucces = (await storage.AddOrder(order));
            Assert.IsTrue(orderpostsucces);
        }

        foreach (var order in orders)
        {
            List<OrderItems?> FoundOrderitems = (await storage.GetItemsInOrder(order.Id)).ToList()!;
            Assert.IsTrue(FoundOrderitems != null);

            // use this to ignore order of the items
            Assert.IsTrue(new HashSet<OrderItems>(FoundOrderitems!).SetEquals(order.Items));
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestOrdersData), typeof(OrderDBTest))]
    public async Task TestGetShipmentsInOrderss(List<Order> orders, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        // expand this test later when it becomes possible/easy to add multiple orders to a shipment
        OrderStorage storage = new(db);

        foreach (var order in orders)
        {
            // add each order and assert that the order has been added
            bool orderpostsucces = (await storage.AddOrder(order));
            Assert.IsTrue(orderpostsucces);

            if (order.Id == 1)
            {
                List<int> ordersInShipmetns = (await storage.GetOrdersInShipment(1)).ToList();
                Assert.IsTrue(ordersInShipmetns.Contains(1));
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestSingleOrderData), typeof(OrderDBTest))]
    public async Task TestUpdateOrder(List<Order> orders, List<List<Tuple<int, int, int, int, int>>> ListOfexpectedInventories, List<string> NewOrderStatuses)
    {
        // an order can be: {'Pending', 'Packed', 'Shipped', 'Delivered'}
        // tests if updating the order also updates the inventories correctly 

        OrderStorage storage = new(db);

        foreach (var order in orders)
        {
            // add each order and assert that the order has been added
            bool orderpostsucces = (await storage.AddOrder(order));
            Assert.IsTrue(orderpostsucces);
        }

        for (int i = 0; i < NewOrderStatuses.Count; i++)
        {
            // change orderstatus and assert inventory amounts
            var NewOrderStatus = NewOrderStatuses[i];
            var expectedInventories = ListOfexpectedInventories[i];

            // update the order
            Order ClonedOrder = ObjectCopier.Clone(orders[0]);
            ClonedOrder.OrderStatus = NewOrderStatus;

            Assert.IsTrue(await storage.UpdateOrder(orders[0].Id, ClonedOrder));
            // Compare expected inventories to actual inventories
            AssertInventoryAmounts(expectedInventories, GetTestInventories());
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestOrdersData), typeof(OrderDBTest))]
    public async Task TestDeleteOrder(List<Order> orders, List<Tuple<int, int, int, int, int>> expectedInventories)
    {
        OrderStorage storage = new(db);

        foreach (var order in orders)
        {
            // add each order and assert that the order has been added
            bool orderpostsucces = (await storage.AddOrder(order));
            Assert.IsTrue(orderpostsucces);
        }

        foreach (var order in orders)
        {
            // add each order and assert that the order has been added
            Assert.IsTrue(await storage.DelteOrder(order.Id));
        }
    }

    [TestMethod]
    public async Task TestShipmentIds()
    {
        // test if adding an order with multiple shipment id's works
        OrderStorage storage = new(db);

        Order testOrder = new()
        {
            Id = 10,
            ShipmentIds = new List<ShipmentsInOrders> { new ShipmentsInOrders(1, 1), new ShipmentsInOrders(1, 2), new ShipmentsInOrders(1, 3) }
        };

        bool orderpostsucces = await storage.AddOrder(testOrder);
        Assert.IsTrue(orderpostsucces);

        Order? FoundOrder = await storage.GetOrder(testOrder.Id);
        Assert.IsNotNull(FoundOrder);

        Assert.AreEqual(3, FoundOrder.ShipmentIds.Count());
    }

    [TestMethod]
    public async Task TestAddingShipmentIds()
    {
        OrderStorage storage = new(db);

        Order testOrder = new()
        {
            Id = 1,
            ShipmentIds = new List<ShipmentsInOrders> { new ShipmentsInOrders(1, 1) }
        };

        Order updatedTestOrder = new()
        {
            Id = 1,
            ShipmentIds = new List<ShipmentsInOrders> { new ShipmentsInOrders(1, 1), new ShipmentsInOrders(1, 2), new ShipmentsInOrders(1, 3) }
        };

        bool orderpostsucces = await storage.AddOrder(testOrder);
        Assert.IsTrue(orderpostsucces);

        bool orderupdatesucces = await storage.UpdateOrder(updatedTestOrder.Id, updatedTestOrder);
        Assert.IsTrue(orderupdatesucces);

        Order? FoundOrder = await storage.GetOrder(testOrder.Id);
        Assert.IsNotNull(FoundOrder);

        Assert.AreEqual(3, FoundOrder.ShipmentIds.Count());
    }

    public static IEnumerable<object[]> TestGetOrdersTestDataPagination => new List<object[]>
    {
    new object[] { Enumerable.Range(1, 0).Select(id => new Order { Id = id }).ToList(), 0, 5 },  //   0 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Order { Id = id }).ToList(), 0, 5 }, //   0 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Order { Id = id }).ToList(), 5, 5 }, //   5 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Order { Id = id }).ToList(), 8, 5 }, //   8 offset, limit 5
    new object[] { Enumerable.Range(1, 10).Select(id => new Order { Id = id }).ToList(), 10, 5 }  //  10 offset, limit 5
    };

    [TestMethod]
    [DynamicData(nameof(TestGetOrdersTestDataPagination), DynamicDataSourceType.Property)]
    public async Task TestGetOrdersWithPagination(List<Order> orders, int offset, int limit)
    {
        // Arrange
        await db.Orders.AddRangeAsync(orders); // Add the test data
        await db.SaveChangesAsync();

        OrderStorage storage = new(db);

        // Act
        IEnumerable<Order> x = await storage.GetOrders(offset, limit);
        List<Order> result = x.ToList();

        // Console.WriteLine($"offset: {offset}  limit:{limit}  count in db:{orders.Count()}  result count:{result.Count()}");
        // foreach (Order location in result)
        // {
        //     Console.WriteLine("Location: " + location.Id);
        // }

        // Assert
        int expectedCount = Math.Min(limit, Math.Max(0, orders.Count - offset));
        Assert.AreEqual(expectedCount, result.Count, "Returned result count is incorrect.");

        for (int i = 0; i < result.Count; i++)
        {
            Assert.AreEqual(orders[offset + i].Id, result[i].Id, "Order ID does not match at index " + i);
        }
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
                ShipmentIds = new List<ShipmentsInOrders> { new ShipmentsInOrders(1, 1) },
                TotalAmount = 6182.77f,
                TotalDiscount = 401.42f,
                TotalTax = 780.29f,
                TotalSurcharge = 85.5f,
                CreatedAt = DateTime.Parse("1971-11-25T18:25:07Z"),
                UpdatedAt = DateTime.Parse("1971-11-27T14:25:07Z"),
                Items = new List<OrderItems>
                {
                    new OrderItems("P000001", 20, 1)
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
                ShipmentIds = new List<ShipmentsInOrders> { new ShipmentsInOrders(2, 1) },
                TotalAmount = 1520.50f,
                TotalDiscount = 100.00f,
                TotalTax = 120.75f,
                TotalSurcharge = 50.0f,
                CreatedAt = DateTime.Parse("1980-05-15T10:15:30Z"),
                UpdatedAt = DateTime.Parse("1980-05-16T11:00:00Z"),
                Items = new List<OrderItems>
                {
                    new OrderItems("P000002", 10, 2),
                    new OrderItems("P000003", 5, 2)
                }
            }
        },
        // Expected inventory values for the second set of inventories
        new List<Tuple<int, int, int, int, int>> {
            new Tuple<int, int, int, int, int>(100, 0, 0, 0, 100), // (Total On Hand, Total Expected, Total Ordered, Total Allocated, Total Available)
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
                ShipmentIds = new List<ShipmentsInOrders> { new ShipmentsInOrders(3, 1) },
                TotalAmount = 850.25f,
                TotalDiscount = 50.00f,
                TotalTax = 68.75f,
                TotalSurcharge = 25.0f,
                CreatedAt = DateTime.Parse("1990-12-01T08:00:00Z"),
                UpdatedAt = DateTime.Parse("1990-12-03T10:30:00Z"),
                Items = new List<OrderItems>
                {
                    new OrderItems("P000001", 8, 3),
                    new OrderItems("P000002", 2, 3)
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
                ShipmentIds = new List<ShipmentsInOrders> { new ShipmentsInOrders(4, 1) },
                TotalAmount = 3200.00f,
                TotalDiscount = 300.00f,
                TotalTax = 200.00f,
                TotalSurcharge = 75.0f,
                CreatedAt = DateTime.Parse("2000-01-10T12:30:00Z"),
                UpdatedAt = DateTime.Parse("2000-01-13T09:00:00Z"),
                Items = new List<OrderItems>
                {
                    new OrderItems("P000002", 15, 3),
                    new OrderItems("P000003", 20, 3)
                }
            }
        },
            // Expected inventory values for the third set of inventories
            new List<Tuple<int, int, int, int, int>> {
            new Tuple<int, int, int, int, int>(100, 0, 0, 8, 92), // (Total On Hand, Total Expected, Total Ordered, Total Allocated, Total Available)
            new Tuple<int, int, int, int, int>(50, 0, 15, 2, 33),
            new Tuple<int, int, int, int, int>(5, 0, 20, 0, -15)
            }
    }
    };

    public static IEnumerable<object[]> TestSingleOrderData => new List<object[]>
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
                    OrderStatus = "Pending",
                    Notes = "Licht gebruikelijk melk brug.",
                    ShippingNotes = "Afmaken thee fris.",
                    PickingNotes = "Meisje vis volgende overal hallo vrijheid gebeurtenis hut.",
                    WareHouseId = 1,
                    ShipTo = 1,
                    BillTo = 1,
                    ShipmentIds = new List<ShipmentsInOrders> { new ShipmentsInOrders(1, 1) },
                    TotalAmount = 6182.77f,
                    TotalDiscount = 401.42f,
                    TotalTax = 780.29f,
                    TotalSurcharge = 85.5f,
                    CreatedAt = DateTime.Parse("1971-11-25T18:25:07Z"),
                    UpdatedAt = DateTime.Parse("1971-11-27T14:25:07Z"),
                    Items = new List<OrderItems>
                    {
                        new OrderItems("P000001", 10, 1),
                        new OrderItems("P000002", 20, 1),
                        new OrderItems("P000003", 5, 1)
                    }
                }
            },
        new List<List<Tuple<int, int, int, int, int>>> {
            // Pending
            new List<Tuple<int, int, int, int, int>> {
                new Tuple<int, int, int, int, int>(100, 0, 0, 10, 90), // (Total On Hand, Total Expected, Total Ordered, Total Allocated, Total Available)
                new Tuple<int, int, int, int, int>(50, 0, 0, 20, 30),
                new Tuple<int, int, int, int, int>(5, 0, 0, 5, 0)
            },
            // Packed
            new List<Tuple<int, int, int, int, int>> {
                new Tuple<int, int, int, int, int>(100, 0, 10, 0, 90),
                new Tuple<int, int, int, int, int>(50, 0, 20, 0, 30),
                new Tuple<int, int, int, int, int>(5, 0, 5, 0, 0)
            },
            // Shipped
            new List<Tuple<int, int, int, int, int>> {
                new Tuple<int, int, int, int, int>(90, 0, 0, 0, 90),
                new Tuple<int, int, int, int, int>(30, 0, 0, 0, 30),
                new Tuple<int, int, int, int, int>(0, 0, 0, 0, 0)
            },
            // Delivered
            new List<Tuple<int, int, int, int, int>> {
                new Tuple<int, int, int, int, int>(90, 0, 0, 0, 90),
                new Tuple<int, int, int, int, int>(30, 0, 0, 0, 30),
                new Tuple<int, int, int, int, int>(0, 0, 0, 0, 0)
            }
        }
,

            new List<string> {"Pending", "Packed", "Shipped", "Delivered"}
        }
    };


    private List<Inventory> GetTestInventories(string type = "")
    {
        // Get all the test inventories
        Inventory? inventory1 = db.Inventories.FirstOrDefault(x => x.Id == 1);
        Inventory? inventory2 = db.Inventories.FirstOrDefault(x => x.Id == 2);
        Inventory? inventory3 = db.Inventories.FirstOrDefault(x => x.Id == 3);
        Assert.IsNotNull(inventory1);
        Assert.IsNotNull(inventory2);
        Assert.IsNotNull(inventory3);

        Console.WriteLine($"--------------- Inventories for test {type} ------------------");
        // Console.WriteLine($"Total On Hand: {inventory1.total_on_hand}, Total Expected: {inventory1.total_expected}, Total Ordered: {inventory1.total_ordered}, Total Allocated: {inventory1.total_allocated}, Total Available: {inventory1.total_available}");
        // Console.WriteLine($"Total On Hand: {inventory2.total_on_hand}, Total Expected: {inventory2.total_expected}, Total Ordered: {inventory2.total_ordered}, Total Allocated: {inventory2.total_allocated}, Total Available: {inventory2.total_available}");
        // Console.WriteLine($"Total On Hand: {inventory3.total_on_hand}, Total Expected: {inventory3.total_expected}, Total Ordered: {inventory3.total_ordered}, Total Allocated: {inventory3.total_allocated}, Total Available: {inventory3.total_available}");

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
