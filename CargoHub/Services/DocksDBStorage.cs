using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class DockDBStorage : IDocksStorage  
{
    private readonly DatabaseContext db;

    public DockDBStorage(DatabaseContext db)  
    {
        this.db = db;
    }

    public async Task<IEnumerable<Dock>> GetAllDocksAsync()
    {
        return await db.Docks
            .Include(d => d.Transfers)
            .Where(d => !d.isDeleted)
            .ToListAsync();
    }

    public async Task<Dock?> GetDockByIdAsync(int id)
    {
        return await db.Docks
            .Include(d => d.Transfers)
            .FirstOrDefaultAsync(d => d.Id == id && !d.isDeleted);
    }

    public async Task<bool> CreateDockAsync(Dock dock)
    {
        await db.Docks.AddAsync(dock);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteDockAsync(int id)
    {
        var dock = await db.Docks.FirstOrDefaultAsync(d => d.Id == id);
        if (dock == null) return false;

        dock.isDeleted = true;
        await db.SaveChangesAsync();
        return true;
    }
}