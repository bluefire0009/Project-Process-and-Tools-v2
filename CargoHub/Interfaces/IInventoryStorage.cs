using System.Collections;
using CargoHub.Models;

public interface IInventoryStorage
{
    Task<IEnumerable<Inventory>> getInventories();
    Task<IEnumerable<Inventory>> GetInventoriesInPagination(int offset, int limit);
    Task<Inventory?> getInventory(int id);
    Task<bool> addInventory(Inventory inventory);
    Task<bool> deleteInventory(int id);
    Task<bool> updateInventory(int id, Inventory? inventory);
}