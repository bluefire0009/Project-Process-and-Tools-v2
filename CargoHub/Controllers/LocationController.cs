using System.Diagnostics.CodeAnalysis;
using CargoHub.Models;
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
    public async Task<IActionResult> GetLocations()
    {
        IEnumerable<Location> suppliers = await Storage.GetLocations();
        return Ok(suppliers);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetLocation([FromRoute] int Id)
    {
        Location? location = await Storage.GetLocation(Id);
        if (location == null) return NotFound();
        return Ok(location);
    }

    [HttpPost("")]
    public async Task<IActionResult> AddLocation([FromBody]Location location)
    {
        if (await Storage.AddLocation(location)) return Ok("Loaction added");
        return BadRequest();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateLocation([FromRoute] int Id, [FromBody] Location location)
    {
        if (await Storage.UpdateLocation(Id, location)) return Ok($"Location with Id{Id} was updated successfully");
        return BadRequest();
    }


}
