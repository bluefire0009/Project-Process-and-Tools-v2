using CargoHub.Models;
public interface IItemStorage {
    Task<List<Item>> GetItems(int offset, int limit);
    Task<Item?> GetItem(string uid);
    Task<List<Inventory>> GetItemInventory(string uid);
    Task<bool> AddItem(Item item);
    Task<bool> DeleteItem(string uid);
    Task<bool> UpdateItem(string uid, Item item);
}