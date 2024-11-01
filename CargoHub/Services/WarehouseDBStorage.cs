using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class WarehouseDBStorage : IWarehouseStorage
{
    DatabaseContext db;
    public WarehouseDBStorage(DatabaseContext db)
    {
        this.db = db;
    }
    public Task<bool> addWarehouse(Warehouse warehouse)
    {
        throw new NotImplementedException();
    }

    public Task<bool> deleteWarehouse(int id)
    {
        throw new NotImplementedException();
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

    public Task<bool> updateWarehouse(int id, Warehouse warehouse)
    {
        throw new NotImplementedException();
    }
}