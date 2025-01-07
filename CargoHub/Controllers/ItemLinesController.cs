using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/itemlines")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class ItemLinesController : Controller {
    private IItemLineStorage ItemLineStorage;

    public ItemLinesController(IItemLineStorage itemLineStorage) {
        ItemLineStorage = itemLineStorage;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItemLines([FromQuery] int offset = 0, [FromQuery] int limit = 100) {
        List<ItemLine> itemLines = await ItemLineStorage.GetItemLines(offset, limit);
        return Ok(itemLines);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemLine(int id) {
        if (id < 0) return BadRequest("invalid uid");
        ItemLine? itemLine = await ItemLineStorage.GetItemLine(id);

        if (itemLine == null) return NotFound($"No item with id:{id} found");
        return Ok(itemLine);
    }

    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItemLineItems(int id) {
        if (id < 0) return BadRequest("invalid id");
        List<Item> itemInventories = await ItemLineStorage.GetItemLineItems(id);

        return Ok(itemInventories);
    }

    [HttpPost()]
    public async Task<IActionResult> AddItemLine([FromBody] ItemLine itemLine) {
        if (itemLine == null) return BadRequest("given ItemLine was null");

        bool added = await ItemLineStorage.AddItemLine(itemLine);

        if (!added) return BadRequest($"Couldn't add item:{JsonConvert.SerializeObject(itemLine)}");
        return Ok("Item has been created");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveItem(int id) {
        if (id < 0) return BadRequest("invalid id");

        bool removed = await ItemLineStorage.DeleteItemLine(id);
        if (!removed) return BadRequest($"Couldn't remove item with id {id}");
        return Ok("Item has been created");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItemLine([FromRoute] int id, [FromBody] ItemLine itemLine) {
        if (id < 0) return BadRequest("invalid id");
        if (itemLine.Id != id) return BadRequest("id does not line up");

        ItemLine? existingItemLine = await ItemLineStorage.GetItemLine(id);
        if (existingItemLine is null) return NotFound($"Item with uid:{id} not found");

        bool updated = await ItemLineStorage.UpdateItemLine(id, itemLine);
        if (!updated) return NotFound($"No item with uid:{id} in the database");

        return Ok($"Updated warhouse id:{id} to:{itemLine}");
    }
}