using System.Diagnostics.CodeAnalysis;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc;


[Route("/api/v2/shipments")]
[ExcludeFromCodeCoverage]
public class ShipmentController : Controller
{
    private IShipmentStorage ShipmentStorage;
    private IOrderStorage OrderStorage;

    public ShipmentController(IShipmentStorage ShipmentStorage, IOrderStorage OrderStorage)
    {
        this.ShipmentStorage = ShipmentStorage;
        this.OrderStorage = OrderStorage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetShipments([FromQuery] int offset = 0, [FromQuery] int limit = 100, [FromQuery] bool orderById = false)
    {
        IEnumerable<Shipment> shipments = await ShipmentStorage.GetShipments(offset, limit, orderById);
        return Ok(shipments);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetShipment([FromRoute] int Id)
    {
        Shipment? shipment = await ShipmentStorage.GetShipment(Id);
        if (shipment == null) return NotFound();
        return Ok(shipment);
    }

    [HttpGet("{Id}/orders")]
    public async Task<IActionResult> GetorderIdsFromShipment([FromRoute] int Id)
    {
        return Ok(await OrderStorage.GetOrdersInShipment(Id));
    }

    [HttpGet("{Id}/items")]
    public async Task<IActionResult> GetItemsInShipment([FromRoute] int Id)
    {
        return Ok(await ShipmentStorage.GetItemsInShipment(Id));
    }

    [HttpPost("")]
    public async Task<IActionResult> AddShipment([FromBody] Shipment shipment)
    {
        if (await ShipmentStorage.AddShipment(shipment)) return Ok("shipment added");
        return BadRequest();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateShipment([FromRoute] int Id, [FromBody] Shipment shipment)
    {
        if (await ShipmentStorage.UpdateShipment(Id, shipment)) return Ok($"Shipment with Id{Id} was updated successfully");
        return BadRequest();
    }

    [HttpPut("{Id}/items")]
    public async Task<IActionResult> UpdateItemsInShipment([FromRoute] int Id, [FromBody] List<ShipmentItems> items)
    {
        if (await ShipmentStorage.UpdateItemsInShipment(Id, items)) return Ok("Items Updated");
        return BadRequest();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DelteShipment([FromRoute] int Id)
    {
        if (await ShipmentStorage.DelteShipment(Id)) return Ok($"Shipment with Id:{Id} Delted");
        return BadRequest();
    }
}