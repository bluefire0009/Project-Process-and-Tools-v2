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
}
