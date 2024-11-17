using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/docks")]
// Gets tested in testfile
[ExcludeFromCodeCoverage]
public class DocksController : Controller
{
    private IDocksStorage dockStorage;
    
    public DocksController(IDocksStorage dockStorage)
    {
        this.dockStorage = dockStorage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllDocks()
    {
        List<Dock> docks = (await dockStorage.getDocks()).ToList();
        return Ok(docks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificDock(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Dock? foundDock = await dockStorage.getDock(id);
        if (foundDock == null) return NotFound($"No dock with id:{id} found");

        return Ok(foundDock);
    }

    [HttpGet("{id}/docktransfers")]
    public async Task<IActionResult> GetTransferSpecificDock(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Dock? foundDock = await dockStorage.getDock(id);
        if (foundDock == null) return NotFound($"No dock with id:{id} found");

        List<Transfer> dockTransfer = dockStorage.getDockTransfers(id).ToList();

        return Ok(dockTransfer);
    }

    [HttpPost("")]
    public async Task<IActionResult> PostDock([FromBody] Dock dock)
    {
        bool added = await dockStorage.addDock(dock);

        if (!added) return BadRequest($"Couldn't add dock:{JsonConvert.SerializeObject(dock)}");
        return Ok($"Added dock:{JsonConvert.SerializeObject(dock)}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveDock(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        bool deleted = await dockStorage.deleteDock(id);

        if (!deleted) return NotFound($"No dock with id:{id} in the database");
        return Ok($"Deleted dock with id: {id}");
    }

    [HttpPut("{idToUpdate}")]
    public async Task<IActionResult> UpdateDock(int idToUpdate, [FromBody] Dock updatedDock)
    {
        if (idToUpdate <= 0) return BadRequest("Invalid id in the url");
        if (updatedDock == null) return BadRequest("updatedDock cannot be null");

        bool updated = await dockStorage.updateDock(idToUpdate, updatedDock);

        if (!updated) return NotFound($"No dock with id:{idToUpdate} in the database");
        return Ok($"Updated dock id:{idToUpdate} to:{updatedDock}");
    }
}