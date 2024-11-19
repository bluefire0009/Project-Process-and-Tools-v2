using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class ItemGroupDBStorage : IItemGroupStorage
{
    DatabaseContext db;
    public ItemGroupDBStorage(DatabaseContext db)
    {
        this.db = db;
    }

    public async Task<ItemGroup?> getItemGroup(int id)
    {
        ItemGroup? itemGroup = await db.ItemGroups.Where(i => i.Id == id).FirstOrDefaultAsync();
        return itemGroup;
    }
    public async Task<IEnumerable<ItemGroup>> GetItemGroupsInPagination(int offset, int limit)
    {
        // Fetch locations with pagination
        return await db.ItemGroups
            .Skip(offset) // Skip the first 'offset' items
            .Take(limit)  // Take the next 'limit' items
            .ToListAsync();
    }

    public async Task<IEnumerable<ItemGroup>> getItemGroups()
    {
        IEnumerable<ItemGroup> itemGroups = await db.ItemGroups.ToListAsync();
        return itemGroups;
    }

    public async Task<bool> addItemGroup(ItemGroup itemGroup)
    {
        if (itemGroup == null) return false;
        if (itemGroup.Id <= 0) return false;

        ItemGroup? itemGroupInDatabase = await db.ItemGroups.Where(i => i.Id == itemGroup.Id).FirstOrDefaultAsync();
        if (itemGroupInDatabase != null) return false;

        itemGroup.CreatedAt = DateTime.Now;

        await db.ItemGroups.AddAsync(itemGroup);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> updateItemGroup(int idToUpdate, ItemGroup? updatedItemGroup)
    {
        if (updatedItemGroup == null) return false;
        if (idToUpdate <= 0 || updatedItemGroup.Id <= 0) return false;

        ItemGroup? itemGroupInDatabase = await db.ItemGroups.Where(i => i.Id == idToUpdate).FirstOrDefaultAsync();
        if (itemGroupInDatabase == null) return false;

        db.Remove(itemGroupInDatabase);
        await db.SaveChangesAsync();

        updatedItemGroup.UpdatedAt = DateTime.Now;

        db.Add(updatedItemGroup);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> deleteItemGroup(int id)
    {
        if (id <= 0) return false;

        ItemGroup? itemGroupInDatabase = await db.ItemGroups.Where(i => i.Id == id).FirstOrDefaultAsync();
        if (itemGroupInDatabase == null) return false;

        db.ItemGroups.Remove(itemGroupInDatabase);

        await db.SaveChangesAsync();
        return true;
    }

    public IEnumerable<Item>? getItemGroupItems(int itemGroupId)
    {
        if (itemGroupId <= 0) return null;

        IEnumerable<Item> items = db.Items.Where(l => l.ItemGroup == itemGroupId);
        return items;
    }
}