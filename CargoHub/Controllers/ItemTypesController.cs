using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/itemtypes")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class ItemTypesController : Controller {
    private IItemTypeStorage ItemTypeStorage;

    public ItemTypesController(IItemTypeStorage itemTypeStorage) {
        ItemTypeStorage = itemTypeStorage;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItemTypes() {
        List<ItemType> items = await ItemTypeStorage.GetItemTypes();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemType(int id) {
        if (id < 0) return BadRequest("invalid uid");
        ItemType? itemType = await ItemTypeStorage.GetItemType(id);

        if (itemType == null) return NotFound($"No item type with id:{id} found");
        return Ok(itemType);
    }

    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItemTypeItems(int id) {
        if (id < 0) return BadRequest("invalid id");
        List<Item> itemInventories = await ItemTypeStorage.GetItemTypeItems(id);

        return Ok(itemInventories);
    }

    [HttpPost()]
    public async Task<IActionResult> AddItemType([FromBody] ItemType itemType) {
        if (itemType == null) return BadRequest("given ItemType was null");

        bool added = await ItemTypeStorage.AddItemType(itemType);

        if (!added) return BadRequest($"Couldn't add item type:{JsonConvert.SerializeObject(itemType)}");
        return Ok("Item type has been created");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveItem(int id) {
        if (id < 0) return BadRequest("invalid id");

        bool removed = await ItemTypeStorage.DeleteItemType(id);
        if (!removed) return BadRequest($"Couldn't remove item type with id {id}");
        return Ok("Item type has been created");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItemType([FromRoute] int id, [FromBody] ItemType itemType) {
        if (id < 0) return BadRequest("invalid id");
        if (itemType.Id != id) return BadRequest("id does not type up");

        ItemType? existingItemType = await ItemTypeStorage.GetItemType(id);
        if (existingItemType is null) return NotFound($"Item type with uid:{id} not found");

        bool updated = await ItemTypeStorage.UpdateItemType(id, itemType);
        if (!updated) return NotFound($"No item type with uid:{id} in the database");

        return Ok($"Updated item type id:{id} to:{itemType}");
    }
    [HttpGet("{id}/weight")]
    public async Task<IActionResult> GetItemTypeWeight(int id)
    {
        if (id < 0) return BadRequest("Invalid ID");

        float? weight = await ItemTypeStorage.GetItemTypeWeight(id);

        if (weight == null) return NotFound($"No item type with ID {id} found or weight not set");

        return Ok(weight);
    }
}