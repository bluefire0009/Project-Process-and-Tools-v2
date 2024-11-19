using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/inventories")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class InventoriesController : Controller
{
    private IInventoryStorage inventoryStorage;
    public InventoriesController(IInventoryStorage inventoryStorage)
    {
        this.inventoryStorage = inventoryStorage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllInventories()
    {
        List<Inventory> inventories = (await inventoryStorage.getInventories()).ToList();
        return Ok(inventories);
    }
    [HttpGet("pagination")]
    public async Task<IActionResult> GetInventoriesInPagination([FromQuery] int offset = 0, [FromQuery] int limit = 100)
    {
        var inventories = await inventoryStorage.GetInventoriesInPagination(offset, limit);
        return Ok(inventories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificInventory(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Inventory? foundInventory = await inventoryStorage.getInventory(id);
        if (foundInventory == null) return NotFound($"No inventory with id:{id} found");

        return Ok(foundInventory);
    }

    [HttpPost("")]
    public async Task<IActionResult> PostInventory([FromBody] Inventory inventory)
    {
        bool added = await inventoryStorage.addInventory(inventory);

        if (!added) return BadRequest($"Couldn't add inventory:{JsonConvert.SerializeObject(inventory)}");
        return Ok($"Added Inventory:{JsonConvert.SerializeObject(inventory)}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveInventory(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");
        bool deleted = await inventoryStorage.deleteInventory(id);

        if (!deleted) return NotFound($"No inventory with id:{id} in the database");
        return Ok($"Deleted inventory with id: {id}");
    }

    [HttpPut("{idToUpdate}")]
    public async Task<IActionResult> UpdateInventory(int idToUpdate, [FromBody] Inventory updatedInventory)
    {
        if (idToUpdate <= 0) return BadRequest("Invalid id in the url");
        if (updatedInventory == null) BadRequest("updated Inventory cannot be null");

        bool updated = await inventoryStorage.updateInventory(idToUpdate, updatedInventory);

        if (!updated) return NotFound($"No inventory with id:{idToUpdate} in the database");
        return Ok($"Updated inventory id:{idToUpdate} to:{updatedInventory}");
    }
}