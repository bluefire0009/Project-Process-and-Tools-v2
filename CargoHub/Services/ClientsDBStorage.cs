using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class ClientDBStorage : IClientStorage
{
    DatabaseContext db;
    public ClientDBStorage(DatabaseContext db)
    {
        this.db = db;
    }
    public IEnumerable<Client> getClients()
    {
        IEnumerable<Client> client = db.Clients.AsEnumerable();
        return client;
    }
    public async Task<Client?> getClient(int id)
    {
        Client? client = await db.Clients.Where(s => s.Id == id).FirstOrDefaultAsync();
        return client;
    }

    public Task<bool> addClient(Client client)
    {
        throw new NotImplementedException();
    }

    public Task<bool> deleteClient(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> updateClient(int id, Client? client)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Order>? getClientOrders(int id)
    {
        throw new NotImplementedException();
    }
}