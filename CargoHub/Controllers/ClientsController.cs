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
        List<Client> clients = (await clientStorage.getClients()).ToList();
        return Ok(clients);
    }
    [HttpGet("pagination")]
    public async Task<IActionResult> GetClientsInPagination([FromQuery] int offset = 0, [FromQuery] int limit = 100)
    {
        var clients = await clientStorage.GetClientsInPagination(offset, limit);
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

    [HttpGet("{id}/orders")]
    public async Task<IActionResult> GetOrdersSpecificClient(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the URL");

        Client? foundClient = await clientStorage.getClient(id);
        if (foundClient == null) return NotFound($"No client with id:{id} found");

        List<Order> clientOrders = clientStorage.getClientOrders(id)?.ToList() ?? new List<Order>();

        return Ok(clientOrders);
    }

    [HttpPost("")]
    public async Task<IActionResult> PostClient([FromBody] Client client)
    {
        bool added = await clientStorage.addClient(client);

        if (!added) return BadRequest($"Couldn't add client:{JsonConvert.SerializeObject(client)}");
        return Ok($"Added client:{JsonConvert.SerializeObject(client)}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveClient(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");
        bool deleted = await clientStorage.deleteClient(id);

        if (!deleted) return NotFound($"No client with id:{id} in the database");
        return Ok($"Deleted client with id: {id}");
    }

    [HttpPut("{idToUpdate}")]
    public async Task<IActionResult> UpdateClient(int idToUpdate, [FromBody] Client updatedClient)
    {
        if (idToUpdate <= 0) return BadRequest("Invalid id in the url");
        if (updatedClient == null) BadRequest("updatedClient cannot be null");

        bool updated = await clientStorage.updateClient(idToUpdate, updatedClient);

        if (!updated) return NotFound($"No client with id:{idToUpdate} in the database");
        return Ok($"Updated client id:{idToUpdate} to:{updatedClient}");
    }

}
