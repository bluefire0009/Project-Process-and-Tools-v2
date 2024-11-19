using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/item_groups")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class ItemGroupController : Controller
{
    private IItemGroupStorage itemGroupStorage;
    public ItemGroupController(IItemGroupStorage itemGroupStorage)
    {
        this.itemGroupStorage = itemGroupStorage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllItemGroup()
    {
        List<ItemGroup> itemGroups = (await itemGroupStorage.getItemGroups()).ToList();
        return Ok(itemGroups);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificItemGroup(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        ItemGroup? foundItemGroup = await itemGroupStorage.getItemGroup(id);
        if (foundItemGroup == null) return NotFound($"No itemGroup with id:{id} found");

        return Ok(foundItemGroup);
    }

    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItemsSpecificItemGroup(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the URL");

        ItemGroup? foundItemGroup = await itemGroupStorage.getItemGroup(id);
        if (foundItemGroup == null) return NotFound($"No client with id:{id} found");

        List<Item> clientOrders = itemGroupStorage.getItemGroupItems(id)?.ToList() ?? new List<Item>();

        return Ok(clientOrders);
    }

    [HttpPost("")]
    public async Task<IActionResult> PostItemGroup([FromBody] ItemGroup itemGroup)
    {
        bool added = await itemGroupStorage.addItemGroup(itemGroup);

        if (!added) return BadRequest($"Couldn't add itemGroup:{JsonConvert.SerializeObject(itemGroup)}");
        return Ok($"Added itemGroup:{JsonConvert.SerializeObject(itemGroup)}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveItemGroup(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");
        bool deleted = await itemGroupStorage.deleteItemGroup(id);

        if (!deleted) return NotFound($"No itemGroup with id:{id} in the database");
        return Ok($"Deleted itemGroup with id: {id}");
    }

    [HttpPut("{idToUpdate}")]
    public async Task<IActionResult> UpdateItemGroup(int idToUpdate, [FromBody] ItemGroup updatedItemGroup)
    {
        if (idToUpdate <= 0) return BadRequest("Invalid id in the url");
        if (updatedItemGroup == null) BadRequest("updatedItemGroup cannot be null");

        bool updated = await itemGroupStorage.updateItemGroup(idToUpdate, updatedItemGroup);

        if (!updated) return NotFound($"No itemGroupe with id:{idToUpdate} in the database");
        return Ok($"Updated warhouse id:{idToUpdate} to:{updatedItemGroup}");
    }
}