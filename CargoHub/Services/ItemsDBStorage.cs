using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class ItemsDBStorage : IItemStorage
{
    DatabaseContext db;

    public ItemsDBStorage(DatabaseContext db) {
        this.db = db;
    }

    public async Task<bool> AddItem(Item item)
    {
        if (item == null) return false;
        if (item.Uid == "") return false;

        Item? itemInDb = await db.Items.FirstOrDefaultAsync(_ => _.Uid == item.Uid);
        if (itemInDb != null) return false;
        
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        await db.Items.AddAsync(item);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItem(string uid)
    {
        if (uid == "") return false;

        Item? itemInDb = await db.Items.FirstOrDefaultAsync(_ => _.Uid == uid);
        if (itemInDb == null) return false;

        db.Items.Remove(itemInDb);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<Item?> GetItem(string uid)
    {
        Item? item = await db.Items.FirstOrDefaultAsync(_ => _.Uid == uid);
        return item;
    }

    public async Task<List<Inventory>> GetItemInventory(string uid)
    {
        List<Inventory> inventory = await db.Inventories.Where(_ => _.ItemId == uid).ToListAsync();
        return inventory;
    }

    public async Task<List<Item>> GetItems()
    {
        List<Item> items = await db.Items.ToListAsync();
        return items;
    }

    public async Task<bool> UpdateItem(string uid, Item item)
    {
        if (item == null) return false;
        if (uid == "" || item.Uid == "" || uid != item.Uid) return false;

        Item? itemInDatabase = await db.Items.Where(i => i.Uid == uid).FirstOrDefaultAsync();
        if (itemInDatabase == null) return false;

        item.UpdatedAt = DateTime.Now;

        db.Items.Update(itemInDatabase);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetItemAmountInWarehouse(string itemId, int warehouseId)
    {
        int amountFound = 0;
        List<int> locationsIds = await db.Locations.Where(l => l.WareHouseId == warehouseId).Select(l => l.Id).ToListAsync();
        var inventories = await db.Inventories.Where(i => i.ItemId == itemId && i.InventoryLocations.Select(il => il.LocationId).Any(id => locationsIds.Contains(id))).ToListAsync();
        foreach (Inventory inventory in inventories)
            if (inventory.ItemId == itemId) amountFound += inventory.total_on_hand;
        return amountFound;
    }
}