using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/docks")]
[ExcludeFromCodeCoverage]
public class DocksController : Controller
{
    private readonly IDocksStorage dockStorage;
    
    public DocksController(IDocksStorage dockStorage)
    {
        this.dockStorage = dockStorage;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDocks()
    {
        var docks = await dockStorage.GetAllDocksAsync();
        return Ok(docks);
    }
    
     [HttpGet("pagination")]
    public async Task<IActionResult> GetDocksWithPagination(int offset, int limit)
    {
        var docks = await dockStorage.GetDocksWithPaginationAsync(offset, limit);
        return Ok(docks);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDockById(int id)
    {
        var dock = await dockStorage.GetDockByIdAsync(id);
        if (dock == null) return NotFound();
        return Ok(dock);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDock([FromBody] Dock dock)
    {
        var created = await dockStorage.CreateDockAsync(dock);
        if (!created) return BadRequest();
        return CreatedAtAction(nameof(GetDockById), new { id = dock.Id }, dock);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteDock(int id)
    {
        var deleted = await dockStorage.SoftDeleteDockAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}