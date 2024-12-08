using System.Diagnostics.CodeAnalysis;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc;


[Route("/api/v2/orders")]
[ExcludeFromCodeCoverage]
public class OrderController : Controller
{
    private IOrderStorage Storage;

    public OrderController(IOrderStorage storage)
    {
        Storage = storage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetOrders([FromQuery] int offset = 0, [FromQuery] int limit = 100, [FromQuery] bool orderById = false)
    {
        IEnumerable<Order> orders = await Storage.GetOrders(offset, limit, orderById);
        return Ok(orders);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetOrder([FromRoute] int Id)
    {
        Order? order = await Storage.GetOrder(Id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet("{Id}/items")]
    public async Task<IActionResult> GetItemsFromOrder([FromRoute] int Id)
    {
        return Ok(await Storage.GetItemsInOrder(Id));
    }

    [HttpPost("")]
    public async Task<IActionResult> AddOrder([FromBody] Order order)
    {
        if (await Storage.AddOrder(order)) return Ok("Order added");
        return BadRequest();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateOrder([FromRoute] int Id, [FromBody] Order order)
    {
        if (await Storage.UpdateOrder(Id, order)) return Ok($"Order with Id{Id} was updated successfully");
        return BadRequest();
    }

    [HttpPut("{Id}/items")]
    public async Task<IActionResult> UpdateItemsInOrder([FromRoute] int Id, [FromBody] List<OrderItems> items)
    {
        if (await Storage.UpdateItemsInOrder(Id, items)) return Ok("Items Updated");
        return BadRequest();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DelteOrder([FromRoute] int Id)
    {
        if (await Storage.DeleteOrder(Id)) return Ok($"Order with Id:{Id} Delted");
        return BadRequest();
    }


}