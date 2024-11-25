using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/items")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class ItemsController : Controller {
    private IItemStorage ItemStorage;

    public ItemsController(IItemStorage itemStorage) {
        ItemStorage = itemStorage;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAllItems() {
        List<Item> items = await ItemStorage.GetItems();
        return Ok(items);
    }

    [HttpGet("get/{uid}")]
    public async Task<IActionResult> GetItem(string uid) {
        if (uid == "") return BadRequest("invalid uid");
        Item? item = await ItemStorage.GetItem(uid);

        if (item == null) return NotFound($"No item with id:{uid} found");
        return Ok(item);
    }

    [HttpGet("{uid}/iventory")]
    public async Task<IActionResult> GetItemInventory(string uid) {
        if (uid == "") return BadRequest("invalid uid");
        List<Inventory> itemInventories = await ItemStorage.GetItemInventory(uid);

        return Ok(itemInventories);
    }

    [HttpGet("{uid}/inventory/totals")]
    public async Task<IActionResult> GetItemInventoryTotals(string uid) {
        if (uid == "") return BadRequest("invalid uid");
        List<Inventory> itemInventories = await ItemStorage.GetItemInventory(uid);

        Dictionary<string, int> totals = new() {
            { "total_expected", 0 },
            { "total_ordered", 0 },
            { "total_allocated", 0 },
            { "total_available", 0 }
        };

        foreach (Inventory inventory in itemInventories)
        {
            foreach (InventoryLocation location in inventory.InventoryLocations)
            {
                totals["total_available"] += 1;
            }
        }

        return Ok(totals);
    }

    [HttpPost()]
    public async Task<IActionResult> AddItem([FromBody] Item item) {
        if (item == null) return BadRequest("given Item was null");

        bool added = await ItemStorage.AddItem(item);

        if (!added) return BadRequest($"Couldn't add item:{JsonConvert.SerializeObject(item)}");
        return Ok("Item has been created");
    }

    [HttpDelete("{uid}")]
    public async Task<IActionResult> RemoveItem(string uid) {
        if (uid == "") return BadRequest("invalid uid");

        bool removed = await ItemStorage.DeleteItem(uid);
        if (!removed) return BadRequest($"Couldn't remove item with id {uid}");
        return Ok("Item has been created");
    }

    [HttpPut("{uid}")]
    public async Task<IActionResult> UpdateItem([FromRoute] string uid, [FromBody] Item item) {
        if (uid == "") return BadRequest("invalid uid");
        if (item.Uid != uid) return BadRequest("Uid does not line up");

        Item? existingItem = await ItemStorage.GetItem(uid);
        if (existingItem is null) return NotFound($"Item with uid:{uid} not found");

        bool updated = await ItemStorage.UpdateItem(uid, item);
        if (!updated) return NotFound($"No item with uid:{uid} in the database");

        return Ok($"Updated warhouse id:{uid} to:{item}");
    }
}