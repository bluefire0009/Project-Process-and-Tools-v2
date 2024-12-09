using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class ItemTypesDBStorage : IItemTypeStorage
{
    DatabaseContext db;

    public ItemTypesDBStorage(DatabaseContext db) {
        this.db = db;
    }

    public async Task<bool> AddItemType(ItemType itemType)
    {
        if (itemType == null) return false;
        if (itemType.Id < 0) return false;

        ItemType? itemTypeInDb = await db.ItemTypes.FirstOrDefaultAsync(_ => _.Id == itemType.Id);
        if (itemTypeInDb != null) return false;

        itemType.CreatedAt = DateTime.Now;
        itemType.UpdatedAt = DateTime.Now;
        await db.ItemTypes.AddAsync(itemType);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItemType(int id)
    {
        if (id < 0) return false;

        ItemType? itemTypeInDb = await db.ItemTypes.FirstOrDefaultAsync(_ => _.Id == id);
        if (itemTypeInDb == null) return false;

        itemTypeInDb.IsDeleted = true;
        db.ItemTypes.Update(itemTypeInDb);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ItemType?> GetItemType(int id)
    {
        ItemType? itemType = await db.ItemTypes.FirstOrDefaultAsync(_ => _.Id == id);
        return itemType;
    }

    public async Task<List<Item>> GetItemTypeItems(int id)
    {
        List<Item> items = await db.Items.Where(_ => _.ItemType == id).ToListAsync();
        return items;
    }

    public async Task<List<ItemType>> GetItemTypes(int offset, int limit)
    {
        // Fetch locations with pagination
        return await db.ItemTypes
            .Skip(offset) // Skip the first 'offset' items
            .Take(limit)  // Take the next 'limit' items
            .ToListAsync();
    }

    public async Task<bool> UpdateItemType(int id, ItemType itemType)
    {
        if (itemType == null) return false;
        if (id < 0 || id != itemType.Id) return false;

        ItemType? itemTypeInDatabase = await db.ItemTypes.Where(i => i.Id == id).FirstOrDefaultAsync();
        if (itemTypeInDatabase == null) return false;

        db.ItemTypes.Remove(itemTypeInDatabase);
        await db.SaveChangesAsync();

        itemType.UpdatedAt = DateTime.Now;
        db.ItemTypes.Add(itemType);
        await db.SaveChangesAsync();
        return true;
    }
}