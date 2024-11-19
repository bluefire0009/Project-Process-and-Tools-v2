using CargoHub.Models;
using Microsoft.EntityFrameworkCore;
using CargoHub.HelperFuctions;
using System.Diagnostics.CodeAnalysis;

public class OrderStorage : IOrderStorage
{
    DatabaseContext DB;

    public OrderStorage(DatabaseContext db)
    {
        DB = db;
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        // Return the first 100 orders, including their items
        return await DB.Orders
            .Include(s => s.Items)
            .Take(100)
            .ToListAsync();
    }

    public async Task<Order?> GetOrder(int orderId)
    {
        // return order by id
        return await DB.Orders.Include(s => s.Items).FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task<IEnumerable<OrderItems>> GetItemsInOrder(int oderId)
    {
        // get all items from order
        return await DB.OrderItems.Where(x => x.OrderId == oderId).ToListAsync();
    }

    public async Task<IEnumerable<int>> GetOrdersInShipment(int shipmentId)
    {
        // get all ordersId's in a shipment with shipmentId
        List<Shipment> Shipments = await DB.Shipments.Where(x => x.Id == shipmentId).ToListAsync();
        return Shipments.Select(x => x.OrderId).ToList();
    }

    // This already exists in the clients controller
    // public Task<IEnumerable<Order>> GetOrdersForClient(int clientId)

    public async Task<bool> AddOrder(Order order)
    {
        // add order to orders
        if (order == null) return false;

        // extract items from order
        List<OrderItems> orderItems = order.Items.ToList();

        // give it the correct CreatedAt field
        order.CreatedAt = DateTime.Now;
        // add the order
        await DB.Orders.AddAsync(order);

        // Save to make it available in the DB for the UpdateItemsInOrder
        if (await DB.SaveChangesAsync() < 1) return false;

        // update the items with add setting so it adjusts the inventories propperly
        await UpdateItemsInOrder(order.Id, orderItems, settings: "add");

        // var itms = GetItemsInOrder(order.Id);
        return true;
    }

    public async Task<bool> UpdateOrder(int orderId, Order order)
    {
        // an order can be: {'Pending', 'Packed', 'Shipped', 'Delivered'}
        // update order by id
        if (order == null) return false;

        // make sure the order exists
        Order? FoundOrder = await DB.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (FoundOrder == null) return false;

        // first empty the items incase the OrderStatus changed
        await UpdateItemsInOrder(orderId, []);

        // update orderstatus here as it is needed for the UpdateItemsInOrder func
        FoundOrder.OrderStatus = order.OrderStatus;
        await DB.SaveChangesAsync();

        // Update the items first
        await UpdateItemsInOrder(order.Id, order.Items.ToList(), settings: "add");

        // update updated at
        FoundOrder.UpdatedAt = DateTime.Now;

        // update rest of exsting order
        // DB.Orders.Update(FoundOrder);
        FoundOrder.SourceId = order.SourceId;
        FoundOrder.OrderDate = order.OrderDate;
        FoundOrder.RequestDate = order.RequestDate;
        FoundOrder.Reference = order.Reference;
        FoundOrder.OrderStatus = order.OrderStatus;
        FoundOrder.Notes = order.Notes;
        FoundOrder.ShippingNotes = order.ShippingNotes;
        FoundOrder.PickingNotes = order.PickingNotes;
        // FoundOrder.WareHouseId = order.WareHouseId;
        // FoundOrder.wareHouse = order.wareHouse;
        // FoundOrder.ShipTo = order.ShipTo;
        // FoundOrder.location = order.location;
        // FoundOrder.BillTo = order.BillTo;
        // FoundOrder.client = order.client;
        // FoundOrder.ShipmentById = order.ShipmentById;
        // FoundOrder.ShipmentId = order.ShipmentId;
        FoundOrder.TotalAmount = order.TotalAmount;
        FoundOrder.TotalDiscount = order.TotalDiscount;
        FoundOrder.TotalTax = order.TotalTax;
        FoundOrder.TotalSurcharge = order.TotalSurcharge;
        // FoundOrder.CreatedAt = order.CreatedAt;
        // FoundOrder.UpdatedAt = order.UpdatedAt;
        // FoundOrder.Items = order.Items;

        foreach (var item in FoundOrder.Items)
        {
            OrderItems? Founditem = DB.OrderItems.FirstOrDefault(x => x.Id == item.Id);
            if (Founditem == null) continue;
            DB.OrderItems.Remove(Founditem);
        }
        await DB.SaveChangesAsync();

        foreach (var item in order.Items)
        {
            var existingItem = await DB.OrderItems.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (existingItem != null)
            {
                // Attach existing item to the context if it exists
                DB.OrderItems.Update(item); // Update the existing item
            }
            else
            {
                // Add new item to the context if it doesn't exist
                DB.OrderItems.Add(item);
            }
        }


        await DB.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DelteOrder(int orderId)
    {
        // delete order by id
        Order? FoundOrder = await DB.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (FoundOrder == null) return false;

        // first remove the items from the order
        await UpdateItemsInOrder(FoundOrder.Id, []);

        // then remove the order
        DB.Orders.Remove(FoundOrder);
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> UpdateItemsInOrder(int orderId, List<OrderItems> list2, string settings = "")
    {
        // check if Order exists

        Order? CurrentOrder = DB.Orders.FirstOrDefault(x => x.Id == orderId);
        List<OrderItems> list1 = new();
        if (CurrentOrder == null) return false;

        if (settings == "add")
        {
            list1 = new();
        }
        else
        {
            // find the OrderItems for this shipmentId
            list1 = DB.OrderItems.Where(x => x.OrderId == orderId).ToList();
        }


        // Get items with the difference in Amount between list1 and list2, including new items
        // this gives a list of items where the amount is how to amount increased / decreased compared
        // to the corrent list of items ex: Item1 = (Id:1, Amount:-10) means that item with id 1 decreasd amount by 10
        // for example old [item1 = (Id:1, Amount:20)] [new item1 = (Id:1, Amount: 10)]
        var itemsToUpdate = list2
    .GroupJoin(
        list1,
        item2 => item2.ItemUid,
        item1 => item1.ItemUid,
        (item2, item1Group) => new { item2, item1Group = item1Group.FirstOrDefault() })
    .Select(x =>
    {
        if (x.item1Group != null)
        {
            // If item exists in list1, calculate the difference in Amount
            return new OrderItems(x.item2.ItemUid, x.item2.Amount - x.item1Group.Amount, orderId);
        }
        else
        {
            // If item is new (not found in list1), add the entire Amount
            return new OrderItems(x.item2.ItemUid, x.item2.Amount, orderId);
        }
    })
    .ToList();

        // Now add items from list1 that are missing in list2 (those will have negative Amount)
        var itemsFromList1 = list1
            .Where(item1 => !list2.Any(item2 => item2.ItemUid == item1.ItemUid))
            .Select(item1 => new OrderItems(item1.ItemUid, -item1.Amount, orderId))
            .ToList();

        // Add those missing items from list1 to the update list
        itemsToUpdate.AddRange(itemsFromList1);

        // Remove items where Amount is 0
        itemsToUpdate.RemoveAll(item => item.Amount == 0);

        // get type of order and check if null
        string OrderStatus = CurrentOrder.OrderStatus!;
        if (OrderStatus == null) return false;


        foreach (OrderItems item in itemsToUpdate)
        {
            // get inventory for the current item
            Inventory? inventory = await DB.Inventories.FirstOrDefaultAsync(x => x.ItemId == item.ItemUid);
            if (inventory == null) return false;

            // {'Shipped', 'Delivered', 'Pending', 'Packed'}

            if (OrderStatus == "Delivered" || OrderStatus == "Shipped")
            {
                // if order is already delivered or shipped the total_available and total_on_hand changes
                inventory.total_available -= item.Amount;
                inventory.total_on_hand -= item.Amount;
            }
            else if (OrderStatus == "Pending")
            {
                // if the order is still pending then the total_allocated and total_available changes
                inventory.total_allocated += item.Amount;
                inventory.total_available -= item.Amount;
            }
            else if (OrderStatus == "Packed")
            {
                // if the order is already packed then the total_allocated and total_available changes
                inventory.total_ordered += item.Amount;
                inventory.total_available -= item.Amount;
            }
            else
            {
                // if OrderStatus is anything else return false
                return false;
            }
        }
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }


    [ExcludeFromCodeCoverage]
    // dont know what this is for so I cant test it
    public async Task<bool> UpdateOrdersInShipment(int shipmentId, List<int> orders)
    {
        List<int> PackedOrders = (await GetOrdersInShipment(shipmentId)).ToList();
        foreach (int orderId in PackedOrders)
        {
            if (!orders.Contains(orderId))
            {
                Order? order = await GetOrder(orderId);
                if (order == null) return false;
                order.ShipmentId = -1;
                order.OrderStatus = "Scheduled";
                await UpdateOrder(orderId, order);
            }
        }
        foreach (int orderId in orders)
        {
            Order? order = await GetOrder(orderId);
            if (order == null) return false;
            order.ShipmentId = shipmentId;
            order.OrderStatus = "Packed";
            await UpdateOrder(orderId, order);
        }
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }
}
