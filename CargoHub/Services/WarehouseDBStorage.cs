using CargoHub.Models;

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

    public Task<Warehouse?> getWarehouse(int id)
    {
        throw new NotImplementedException();
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