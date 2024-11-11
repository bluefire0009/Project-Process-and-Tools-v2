using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/transfers")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class TransferController : Controller
{
    private ITransferStorage transferStorage;
    public TransferController(ITransferStorage transferStorage)
    {
        this.transferStorage = transferStorage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllTransfers()
    {
        List<Transfer> transfers = (await transferStorage.getTransfers()).ToList();
        return Ok(transfers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificTransfer(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Transfer? foundTransfer = await transferStorage.getTransfer(id);
        if (foundTransfer == null) return NotFound($"No transfer with id:{id} found");

        return Ok(foundTransfer);
    }

    [HttpPost("")]
    public async Task<IActionResult> PostTransfer([FromBody] Transfer transfer)
    {
        bool added = await transferStorage.addTransfer(transfer);

        if (!added) return BadRequest($"Couldn't add transfer:{JsonConvert.SerializeObject(transfer)}");
        return Ok($"Added transfer:{JsonConvert.SerializeObject(transfer)}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveTransfer(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");
        bool deleted = await transferStorage.deleteTransfer(id);

        if (!deleted) return NotFound($"No transfer with id:{id} in the database");
        return Ok($"Deleted transfer with id: {id}");
    }

    [HttpPut("{idToUpdate}")]
    public async Task<IActionResult> UpdateTransfer(int idToUpdate, [FromBody] Transfer updatedTransfer)
    {
        if (idToUpdate <= 0) return BadRequest("Invalid id in the url");
        if (updatedTransfer == null) BadRequest("updatedTransfer cannot be null");

        bool updated = await transferStorage.updateTransfer(idToUpdate, updatedTransfer);

        if (!updated) return NotFound($"No transfer with id:{idToUpdate} in the database");
        return Ok($"Updated transfer id:{idToUpdate} to:{updatedTransfer}");
    }

    [HttpPut("{idToUpdate}/commit")]
    public async Task<IActionResult> CommitTransfer(int idToUpdate)
    {
        if (idToUpdate <= 0) return BadRequest("Invalid id in the url");

        var updated = await transferStorage.commitTransfer(idToUpdate);

        if (!updated.succeded && updated.message == TransferDBStorage.TransferResult.notEnoughItems) return BadRequest($"There are not enough items in the location to carry out the transfer");
        if (!updated.succeded && updated.message == TransferDBStorage.TransferResult.transferNotFound) return NotFound($"No transfer with id:{idToUpdate} in the database");
        if (!updated.succeded && updated.message == TransferDBStorage.TransferResult.FromInventoryNotExsists) return BadRequest($"Inventory to transfer from is not in the database");
        if (!updated.succeded && updated.message == TransferDBStorage.TransferResult.ToInventoryNotExsists) return BadRequest($"Inventory to transfer to is not in the database");

        return Ok($"Committed transfer id:{idToUpdate}");
    }
}