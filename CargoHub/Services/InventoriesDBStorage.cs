using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class InventoriesDBStorage : IInventoryStorage
{
    DatabaseContext db;
    public InventoriesDBStorage(DatabaseContext db)
    {
        this.db = db;
    }

    public IEnumerable<Inventory> getInventories()
    {
        IEnumerable<Inventory> inventories = db.Inventories.AsEnumerable();
        return inventories;
    }

    public async Task<Inventory?> getInventory(int id)
    {
        Inventory? inventory = await db.Inventories.Where(i => i.Id == id).FirstOrDefaultAsync();
        return inventory;
    }

    public async Task<bool> addInventory(Inventory inventory)
    {
        if (inventory == null) return false;
        if (inventory.Id <= 0) return false;

        Inventory? inventoryInDatabase = await db.Inventories.Where(w => w.Id == inventory.Id).FirstOrDefaultAsync(); 
        if (inventoryInDatabase != null) return false;

        await db.Inventories.AddAsync(inventory);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> deleteInventory(int id)
    {
        if (id <= 0) return false;

        Inventory? inventoryInDatabase = await db.Inventories.Where(i => i.Id == id).FirstOrDefaultAsync();
        if (inventoryInDatabase == null) return false;

        db.Inventories.Remove(inventoryInDatabase);
        
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> updateInventory(int idToUpdate, Inventory? updatedInventory)
    {
        if (updatedInventory == null) return false;
        if (idToUpdate <= 0 || updatedInventory.Id <= 0) return false;

        Inventory? inventoryInDatabase = await db.Inventories.Where(w => w.Id == idToUpdate).FirstOrDefaultAsync();
        if (inventoryInDatabase == null) return false;

        db.Remove(inventoryInDatabase);
        await db.SaveChangesAsync();

        db.Add(updatedInventory);
        await db.SaveChangesAsync();

        return true;
    }
}