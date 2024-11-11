using System.Collections;
using CargoHub.Models;

public interface IInventoryStorage
{
    IEnumerable<Inventory> getInventories();
    Task<Inventory?> getInventory(int id);
    Task<bool> addInventory(Inventory inventory);
    Task<bool> deleteInventory(int id);
    Task<bool> updateInventory(int id, Inventory? inventory);
}