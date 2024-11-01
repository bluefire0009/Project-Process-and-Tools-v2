using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class WarehouseDBStorage : IWarehouseStorage
{
    DatabaseContext db;
    public WarehouseDBStorage(DatabaseContext db)
    {
        this.db = db;
    }
    public async Task<bool> addWarehouse(Warehouse warehouse)
    {
        if (warehouse == null) return false;
        if (warehouse.Id <= 0) return false;

        Warehouse? warehouseInDatabase = await db.Warehouses.Where(w => w.Id == warehouse.Id).FirstOrDefaultAsync(); 
        if (warehouseInDatabase != null) return false;

        await db.Warehouses.AddAsync(warehouse);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> deleteWarehouse(int id)
    {
        if (id <= 0) return false;

        Warehouse? warehouseInDatabase = await db.Warehouses.Where(w => w.Id == id).FirstOrDefaultAsync();
        if (warehouseInDatabase == null) return false;

        db.Warehouses.Remove(warehouseInDatabase);
        
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<Warehouse?> getWarehouse(int id)
    {
        Warehouse? warehouse = await db.Warehouses.Where(w => w.Id == id).FirstOrDefaultAsync();
        return warehouse;
    }

    public IEnumerable<Warehouse> getWarehouses()
    {
        IEnumerable<Warehouse> warehouses = db.Warehouses.AsEnumerable();
        return warehouses;
    }

    public IEnumerable<Location>? getWarehouseLocations(int warehouseId)
    {
        if (warehouseId <= 0) return null;
        
        IEnumerable<Location> locations = db.Locations.Where(l => l.WareHouseId == warehouseId);
        return locations;
    }

    public async Task<bool> updateWarehouse(int idToUpdate, Warehouse? updatedWarehouse)
    {
        if (updatedWarehouse == null) return false;
        if (idToUpdate <= 0 || updatedWarehouse.Id <= 0) return false;

        Warehouse? warehouseInDatabase = await db.Warehouses.Where(w => w.Id == idToUpdate).FirstOrDefaultAsync();
        if (warehouseInDatabase == null) return false;

        db.Remove(warehouseInDatabase);
        await db.SaveChangesAsync();

        db.Add(updatedWarehouse);
        await db.SaveChangesAsync();

        return true;
    }
}