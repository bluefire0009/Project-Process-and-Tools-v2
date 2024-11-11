using CargoHub.Models;

public interface IOrderStorage
{
    Task<IEnumerable<Order>> GetOrders();
    Task<Order?> GetOrder(int orderId);
    Task<IEnumerable<OrderItems>> GetItemsInOrder(int oderId);
    Task<IEnumerable<int>> GetOrdersInShipment(int shipmentId);

    // This already exists in the clients controller
    // Task<IEnumerable<Order>> GetOrdersForClient(int clientId)
    Task<bool> AddOrder(Order order);
    Task<bool> UpdateOrder(int orderId, Order order);
    Task<bool> UpdateItemsInOrder(int orderId, List<OrderItems> orderItems);
    Task<bool> UpdateOrdersInShipment(int shipmentId, List<int> orders);
    Task<bool> DelteOrder(int orderId);
}