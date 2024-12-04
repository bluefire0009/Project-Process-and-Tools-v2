using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/warehouses")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class WarehousesController : Controller
{
    private IWarehouseStorage warehouseStorage;
    public WarehousesController(IWarehouseStorage warehouseStorage)
    {
        this.warehouseStorage = warehouseStorage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllWarehouses()
    {
        List<Warehouse> warehouses = (await warehouseStorage.getWarehouses()).ToList();
        return Ok(warehouses);
    }

    [HttpGet("Range")]
    public async Task<IActionResult> GetWarehousesRange([FromQuery]int firstIdToTake, [FromQuery]int amountToTake)
    {
        IEnumerable<Warehouse>? warehouses = await warehouseStorage.getWarehousesRange(firstIdToTake, amountToTake);
        if (warehouses == null) return BadRequest("Invalid firstIdToTake or invalid amountToTake in the url");
        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificWarehouse(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Warehouse? foundWarehouse = await warehouseStorage.getWarehouse(id);
        if (foundWarehouse == null) return NotFound($"No warehouse with id:{id} found");

        return Ok(foundWarehouse);
    }

    [HttpGet("{id}/locations")]
    public async Task<IActionResult> GetLocationsSpecificWarehouse(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Warehouse? foundWarehouse = await warehouseStorage.getWarehouse(id);
        if (foundWarehouse == null) return NotFound($"No warehouse with id:{id} found");

        List<Location> warehouseLocations = warehouseStorage.getWarehouseLocations(id).ToList();

        return Ok(warehouseLocations);
    }

    [HttpPost("")]
    public async Task<IActionResult> PostWarehouse([FromBody] Warehouse warehouse)
    {
        bool added = await warehouseStorage.addWarehouse(warehouse);

        if (!added) return BadRequest($"Couldn't add warehouse:{JsonConvert.SerializeObject(warehouse)}");
        return Ok($"Added warehouse:{JsonConvert.SerializeObject(warehouse)}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveWarehouse(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");
        bool deleted = await warehouseStorage.deleteWarehouse(id);

        if (!deleted) return NotFound($"No warehouse with id:{id} in the database");
        return Ok($"Deleted warehouse with id: {id}");
    }

    [HttpPut("{idToUpdate}")]
    public async Task<IActionResult> UpdateWarehouse(int idToUpdate, [FromBody] Warehouse updatedWarehouse)
    {
        if (idToUpdate <= 0) return BadRequest("Invalid id in the url");
        if (updatedWarehouse == null) BadRequest("updatedWarehouse cannot be null");

        bool updated = await warehouseStorage.updateWarehouse(idToUpdate, updatedWarehouse);

        if (!updated) return NotFound($"No warehouse with id:{idToUpdate} in the database");
        return Ok($"Updated warhouse id:{idToUpdate} to:{updatedWarehouse}");
    }
}