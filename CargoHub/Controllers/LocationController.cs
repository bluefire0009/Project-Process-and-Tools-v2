using System.Diagnostics.CodeAnalysis;
using CargoHub.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


[Route("/api/v2/locations")]
[ExcludeFromCodeCoverage]
public class LocationController : Controller
{
    private ILocationStorage Storage;

    public LocationController(ILocationStorage storage)
    {
        Storage = storage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetLocations([FromQuery] int offset = 0, [FromQuery] int limit = 100, [FromQuery] bool orderById = false)
    {
        IEnumerable<Location> locations = await Storage.GetLocations(offset, limit, orderById);
        return Ok(locations);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetLocation([FromRoute] int Id)
    {
        Location? location = await Storage.GetLocation(Id);
        if (location == null) return NotFound();
        return Ok(location);
    }

    [HttpPost("")]
    public async Task<IActionResult> AddLocation([FromBody] Location location)
    {
        if (location == null) return BadRequest("Location is null");
        int id = await Storage.AddLocation(location);
        if (id == -1) return BadRequest();
        return Ok(new { id });
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateLocation([FromRoute] int Id, [FromBody] Location location)
    {
        if (await Storage.UpdateLocation(Id, location)) return Ok($"Location with Id{Id} was updated successfully");
        return BadRequest();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DelteLocation([FromRoute] int Id)
    {
        if (await Storage.DeleteLocation(Id)) return Ok($"Location with Id:{Id} Delted");
        return BadRequest();
    }
}
