using CargoHub.Models;

public interface ILocationStorage
{
    Task<IEnumerable<Location>> GetLocations();
    Task<IEnumerable<Location>> GetLocations(int offset, int limit);
    Task<Location?> GetLocation(int locationId);
    Task<IEnumerable<Location>> GetLocationsInWarehouses(int warehouseId);
    Task<bool> AddLocation(Location location);
    Task<bool> UpdateLocation(int locationId, Location location);
    Task<bool> DeleteLocation(int locationId);
}