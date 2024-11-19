using CargoHub.Models;
public interface IItemTypeStorage {
    Task<List<ItemType>> GetItemTypes();
    Task<ItemType?> GetItemType(int id);
    Task<List<Item>> GetItemTypeItems(int id);
    Task<bool> AddItemType(ItemType itemType);
    Task<bool> DeleteItemType(int id);
    Task<bool> UpdateItemType(int id, ItemType itemType);
}