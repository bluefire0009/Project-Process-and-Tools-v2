using CargoHub.Models;
using Microsoft.EntityFrameworkCore;


public class LocationStroage : ILocationStorage
{
    DatabaseContext DB;

    private readonly int _maxItemsLimit;

    public LocationStroage(DatabaseContext db, IConfiguration configuration)
    {
        DB = db;
        _maxItemsLimit = configuration.GetValue<int>("MaxResultsLimit");
    }

    public int MaxItemsLimit()
    {
        return _maxItemsLimit;
    }

    public async Task<IEnumerable<Location>> GetLocations()
    {
        // retun first 100 locations
        return await DB.Locations.Take(100).ToListAsync();
    }
    public async Task<Location?> GetLocation(int locationId)
    {
        // return location by id
        return await DB.Locations.FirstOrDefaultAsync(x => x.Id == locationId);
    }

    public async Task<IEnumerable<Location>> GetLocationsInWarehouses(int GivenWarehouseId)
    {
        // find all locations with the given WarehouseId
        // currently not used by location controller
        return await DB.Locations.Where(x => x.WareHouseId == GivenWarehouseId).ToListAsync();
    }

    public async Task<bool> AddLocation(Location location)
    {
        // add location to Locations
        if (location == null) return false;

        location.CreatedAt = DateTime.Now;

        await DB.Locations.AddAsync(location);
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> UpdateLocation(int locationId, Location location)
    {
        // update location by id
        if (location == null) return false;

        Location? Foundlocation = await DB.Locations.FirstOrDefaultAsync(x => x.Id == locationId);
        if (Foundlocation == null) return false;

        // make sure the id doesnt get changed
        location.Id = locationId;
        // update updated at
        location.UpdatedAt = DateTime.Now;

        // update exsting location
        DB.Locations.Update(location);

        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> DeleteLocation(int locationId)
    {
        // delete location by id
        Location? Foundlocation = await DB.Locations.FirstOrDefaultAsync(x => x.Id == locationId);
        if (Foundlocation == null) return false;

        DB.Locations.Remove(Foundlocation);
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }
}