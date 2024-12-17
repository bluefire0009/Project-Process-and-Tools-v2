using CargoHub.HelperFuctions;
using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class InventoriesDBStorage : IInventoryStorage
{
    DatabaseContext db;
    public InventoriesDBStorage(DatabaseContext db)
    {
        this.db = db;
    }

    public async Task<IEnumerable<Inventory>> getInventories()
    {
        IEnumerable<Inventory> inventories = await db.Inventories.ToListAsync();
        return inventories;
    }
    public async Task<IEnumerable<Inventory>> GetInventoriesInPagination(int offset, int limit)
    {
        // Fetch locations with pagination
        return await db.Inventories
            .Skip(offset) // Skip the first 'offset' items
            .Take(limit)  // Take the next 'limit' items
            .ToListAsync();
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

        inventory.CreatedAt = CETDateTime.Now();
        inventory.UpdatedAt = CETDateTime.Now();

        await db.Inventories.AddAsync(inventory);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> deleteInventory(int id)
    {
        if (id <= 0) return false;

        Inventory? inventoryInDatabase = await db.Inventories.Where(i => i.Id == id).FirstOrDefaultAsync();
        if (inventoryInDatabase == null) return false;

        inventoryInDatabase.IsDeleted = true;
        db.Inventories.Update(inventoryInDatabase);

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

        updatedInventory.UpdatedAt = DateTime.Now;

        db.Add(updatedInventory);
        await db.SaveChangesAsync();

        return true;
    }
}