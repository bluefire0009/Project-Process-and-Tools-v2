using CargoHub.Models;
using Microsoft.EntityFrameworkCore;


public class OrderStroage : IOrderStorage
{
    DatabaseContext DB;

    public OrderStroage(DatabaseContext db)
    {
        DB = db;
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        // return all orders
        return await DB.Orders.ToListAsync();
    }

    public async Task<Order?> GetOrder(int orderId)
    {
        // return order by id
        return await DB.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
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
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<bool> AddOrder(Order order)
    {
        // add order to orders
        if (order == null) return false;

        order.CreatedAt = DateTime.Now;

        await DB.Orders.AddAsync(order);
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> UpdateOrder(int orderId, Order order)
    {
        // update order by id
        if (order == null) return false;

        // make sure the order exists
        Order? FoundOrder = await DB.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (FoundOrder == null) return false;

        // make sure the id doesnt get changed
        order.Id = orderId;
        // update updated at
        order.UpdatedAt = DateTime.Now;

        // update exsting order
        DB.Orders.Update(order);

        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> DelteOrder(int orderId)
    {
        // delete order by id
        Order? FoundOrder = await DB.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (FoundOrder == null) return false;

        DB.Orders.Remove(FoundOrder);
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> UpdateItemsInOrder(int orderId, List<OrderItems> list2)
    {
        // check if Order exists
        Order? CurrentOrder = DB.Orders.FirstOrDefault(x => x.Id == orderId);
        if (CurrentOrder == null) return false;

        // find the OrderItems for this shipmentId
        List<OrderItems> list1 = DB.OrderItems.Where(x => x.OrderId == orderId).ToList();

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
                    return new OrderItems(x.item2.ItemUid, x.item2.Amount - x.item1Group.Amount);
                }
                else
                {
                    // If item is new (not found in list1), add the entire Amount
                    return new OrderItems(x.item2.ItemUid, x.item2.Amount);
                }
            })
            .ToList();
        // remove all items form list where amount didnt change
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
                inventory.total_available += item.Amount;
                inventory.total_on_hand += item.Amount;
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
