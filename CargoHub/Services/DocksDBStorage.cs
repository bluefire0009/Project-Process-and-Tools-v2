using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class DockDBStorage : IDocksStorage  
{
    DatabaseContext db;

    public DockDBStorage(DatabaseContext db)  
    {
        this.db = db;
    }

    // Get all Docks 
    public async Task<IEnumerable<Dock>> getDocks()
    {
        List<Dock> docks = await db.Docks.ToListAsync(); 
        return docks;
    }

    // Get specific Dock by ID
    public async Task<Dock?> getDock(int id)
    {
        Dock? dock = await db.Docks.Where(d => d.Id == id).FirstOrDefaultAsync();  
        return dock;
    }

    // Get transfers associated with a specific Dock 
    public IEnumerable<Transfer>? getDockTransfers(int dockTransferID)
    {
        if (dockTransferID <= 0) return null;

        IEnumerable<Transfer> DockTransfers = db.Transfers.Where(l => l.Id == dockTransferID); 
        return DockTransfers;
    }

    // Add a new Dock
    public async Task<bool> addDock(Dock dock)
    {
        if (dock == null) return false;
        if (dock.Id <= 0) return false;

        Dock? dockInDatabase = await db.Docks.Where(d => d.Id == dock.Id).FirstOrDefaultAsync(); 
        if (dockInDatabase != null) return false;

        await db.Docks.AddAsync(dock);  

        await db.SaveChangesAsync();
        return true;
    }

    // Delete an existing Dock
    public async Task<bool> deleteDock(int id)
    {
        if (id <= 0) return false;

        Dock? dockInDatabase = await db.Docks.Where(d => d.Id == id).FirstOrDefaultAsync(); 
        if (dockInDatabase == null) return false;

        db.Docks.Remove(dockInDatabase);  

        await db.SaveChangesAsync();
        return true;
    }

    // Update an existing Dock
    public async Task<bool> updateDock(int idToUpdate, Dock? updatedDock)
    {
        if (updatedDock == null) return false;
        if (idToUpdate != updatedDock.Id) return false;
        if (idToUpdate <= 0 || updatedDock.Id <= 0) return false;

        Dock? dockInDatabase = await db.Docks.Where(d => d.Id == idToUpdate).FirstOrDefaultAsync();  
        if (dockInDatabase == null) return false;

        db.Remove(dockInDatabase);
        await db.SaveChangesAsync();

        db.Add(updatedDock);
        await db.SaveChangesAsync();

        return true;
    }
}