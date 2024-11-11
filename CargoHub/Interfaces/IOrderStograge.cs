using CargoHub.Models;

public interface IOrderStorage
{
    Task<IEnumerable<Order>> GetOrders();
    Task<Order?> GetOrder(int orderId);
    Task<IEnumerable<OrderItems>> GetItemsInOrder(int oderId);
    Task<IEnumerable<int>> GetOrdersInShipment(int shipmentId);
    Task<bool> AddOrder(Order order);
    Task<bool> UpdateOrder(int orderId, Order order);
    Task<bool> DelteOrder(int orderId);
}