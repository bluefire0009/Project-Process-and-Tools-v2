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
    public async Task<IActionResult> GetAllItemTypes([FromQuery] int offset = 0, [FromQuery] int limit = 100) {
        List<ItemType> itemTypes = await ItemTypeStorage.GetItemTypes(offset, limit);
        return Ok(itemTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemType(int id) {
        if (id < 0) return BadRequest("invalid id");
        ItemType? itemType = await ItemTypeStorage.GetItemType(id);

        if (itemType == null) return NotFound($"No itemType with id:{id} found");
        return Ok(itemType);
    }

    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItemTypeItems(int id) {
        if (id < 0) return BadRequest("invalid id");
        List<Item> items = await ItemTypeStorage.GetItemTypeItems(id);

        return Ok(items);
    }

    [HttpPost()]
    public async Task<IActionResult> AddItemType([FromBody] ItemType itemType) {
        if (itemType == null) return BadRequest("given ItemType was null");

        bool added = await ItemTypeStorage.AddItemType(itemType);

        if (!added) return BadRequest($"Couldn't add itemType :{JsonConvert.SerializeObject(itemType)}");
        return Created("", "Item Type has been created");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveItemType(int id) {
        if (id < 0) return BadRequest("invalid id");

        bool removed = await ItemTypeStorage.DeleteItemType(id);
        if (!removed) return BadRequest($"Couldn't remove item type with id {id}");
        return Ok("Item Type has been removed");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItemType([FromRoute] int id, [FromBody] ItemType itemType) {
        if (id < 0) return BadRequest("invalid id");
        if (itemType.Id != id) return BadRequest("id does not match");

        ItemType? existingItemType = await ItemTypeStorage.GetItemType(id);
        if (existingItemType is null) return NotFound($"Item with id:{id} not found");

        bool updated = await ItemTypeStorage.UpdateItemType(id, itemType);
        if (!updated) return NotFound($"No item type with id:{id} in the database");

        return Ok($"Updated ItemType id:{id} to:{itemType}");
    }
}