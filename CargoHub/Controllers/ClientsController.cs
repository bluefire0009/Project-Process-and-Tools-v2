using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/clients")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public ClientsController : Controller
{
    private IClientsStorage clientStorage;

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

}
