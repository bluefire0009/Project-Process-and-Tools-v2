using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/clients")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class ClientsController : Controller
{
    private IClientStorage clientStorage;

    public ClientsController(IClientStorage clientStorage)
    {
        this.clientStorage = clientStorage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllClients()
    {
        List<Client> clients = clientStorage.getClients().ToList();
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificClient(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Client? foundClient = await clientStorage.getClient(id);
        if (foundClient == null) return NotFound($"No client with id:{id} found");

        return Ok(foundClient);
    }

}
