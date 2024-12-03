using CargoHub.Models;
public interface IItemLineStorage {
    Task<List<ItemLine>> GetItemLines(int offset, int limit);
    Task<ItemLine?> GetItemLine(int id);
    Task<List<Item>> GetItemLineItems(int id);
    Task<bool> AddItemLine(ItemLine itemLine);
    Task<bool> DeleteItemLine(int id);
    Task<bool> UpdateItemLine(int id, ItemLine itemLine);
}