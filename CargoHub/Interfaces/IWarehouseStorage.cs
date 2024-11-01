using System.Collections;
using CargoHub.Models;

public interface IWarehouseStorage
{
    IEnumerable<Warehouse> getWarehouses();
    Task<Warehouse?> getWarehouse(int id);
    Task<bool> addWarehouse(Warehouse warehouse);
    Task<bool> deleteWarehouse(int id);
    Task<bool> updateWarehouse(int id, Warehouse? warehouse);
    IEnumerable<Location>? getWarehouseLocations(int id);
}