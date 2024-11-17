using CargoHub.Models;
using Microsoft.EntityFrameworkCore;


public class LocationStroage : ILocationStorage
{
    DatabaseContext DB;

    public LocationStroage(DatabaseContext db)
    {
        DB = db;
    }

    public async Task<IEnumerable<Location>> GetLocations()
    {
        // retun all locations
        return await DB.Locations.ToListAsync();
    }
    public async Task<Location?> GetLocation(int locationId)
    {
        // return location by id
        return await DB.Locations.FirstOrDefaultAsync(x => x.Id == locationId);
    }

    public async Task<IEnumerable<Location>> GetLocationsInWarehouses(int GivenWarehouseId)
    {
        // find all location with the given WarehouseId
        // currently not used by location controller
        return await DB.Locations.Where(x => x.WareHouseId == GivenWarehouseId).ToListAsync();
    }

    public async Task<bool> AddLocation(Location location)
    {
        // add location to Locations
        if (location == null) return false;

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
        Foundlocation.Id = locationId;
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