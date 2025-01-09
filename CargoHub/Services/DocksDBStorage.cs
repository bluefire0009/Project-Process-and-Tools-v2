using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class DocksDBStorage : IDocksStorage  
{
    private readonly DatabaseContext db;

    public DocksDBStorage(DatabaseContext db)  
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
        if (dock.Id == 0)
        {
            return false;
        }

        var existingDock = await db.Docks.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dock.Id);
        if (existingDock != null)
        {
            return false;
        }

        await db.Docks.AddAsync(dock);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteDockAsync(int id)
    {
        var dock = await db.Docks.FirstOrDefaultAsync(d => d.Id == id);
        if (dock == null) return false;

        if (dock.isDeleted)
        {
            return false;
        }

        dock.isDeleted = true;
        await db.SaveChangesAsync();
        return true;
    }
}