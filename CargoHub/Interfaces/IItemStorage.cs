using CargoHub.Models;
public interface IItemStorage {
    IEnumerable<Item> GetItems();
    Task<Item?> GetItem(int id);
    Task<bool> AddItem(Item item);
    Task<bool> DeleteItem(int id);
    Task<bool> updateItem(int id, Item item);
}