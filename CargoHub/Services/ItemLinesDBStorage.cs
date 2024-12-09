using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class ItemLinesDBStorage : IItemLineStorage
{
    DatabaseContext db;

    public ItemLinesDBStorage(DatabaseContext db) {
        this.db = db;
    }

    public async Task<bool> AddItemLine(ItemLine itemLine)
    {
        if (itemLine == null) return false;
        if (itemLine.Id < 0) return false;

        ItemLine? itemLineInDb = await db.ItemLines.FirstOrDefaultAsync(_ => _.Id == itemLine.Id);
        if (itemLineInDb != null) return false;

        itemLine.CreatedAt = DateTime.Now;
        itemLine.UpdatedAt = DateTime.Now;
        await db.ItemLines.AddAsync(itemLine);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItemLine(int id)
    {
        if (id < 0) return false;

        ItemLine? itemLineInDb = await db.ItemLines.FirstOrDefaultAsync(_ => _.Id == id);
        if (itemLineInDb == null) return false;

        itemLineInDb.IsDeleted = true;
        db.ItemLines.Update(itemLineInDb);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ItemLine?> GetItemLine(int id)
    {
        ItemLine? itemLine = await db.ItemLines.FirstOrDefaultAsync(_ => _.Id == id);
        return itemLine;
    }

    public async Task<List<Item>> GetItemLineItems(int id)
    {
        List<Item> items = await db.Items.Where(_ => _.ItemLine == id).ToListAsync();
        return items;
    }

    public async Task<List<ItemLine>> GetItemLines(int offset, int limit)
    {
        // Fetch locations with pagination
        return await db.ItemLines
            .Skip(offset) // Skip the first 'offset' items
            .Take(limit)  // Take the next 'limit' items
            .ToListAsync();
    }

    public async Task<bool> UpdateItemLine(int id, ItemLine itemLine)
    {
        if (itemLine == null) return false;
        if (id < 0 || id != itemLine.Id) return false;

        ItemLine? itemLineInDatabase = await db.ItemLines.Where(i => i.Id == id).FirstOrDefaultAsync();
        if (itemLineInDatabase == null) return false;

        itemLine.UpdatedAt = DateTime.Now;
        db.ItemLines.Update(itemLineInDatabase);
        await db.SaveChangesAsync();

        return true;
    }
}