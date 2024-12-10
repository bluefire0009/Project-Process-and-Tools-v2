using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class ClientDBStorage : IClientStorage
{
    DatabaseContext db;
    public ClientDBStorage(DatabaseContext db)
    {
        this.db = db;
    }
    public async Task<IEnumerable<Client>> getClients()
    {
        IEnumerable<Client> client = await db.Clients.ToListAsync();
        return client;
    }
    public async Task<IEnumerable<Client>> GetClientsInPagination(int offset, int limit)
    {
        // Fetch locations with pagination
        return await db.Clients
            .Skip(offset) // Skip the first 'offset' items
            .Take(limit)  // Take the next 'limit' items
            .ToListAsync();
    }
    public async Task<Client?> getClient(int id)
    {
        Client? client = await db.Clients.Where(s => s.Id == id).FirstOrDefaultAsync();
        return client;
    }
    public async Task<bool> addClient(Client client)
    {
        if (client == null) return false;
        if (client.Id <= 0) return false;

        Client? clientInDatabase = await db.Clients.Where(c => c.Id == client.Id).FirstOrDefaultAsync();
        if (clientInDatabase != null) return false;

        client.CreatedAt = DateTime.Now;
        await db.Clients.AddAsync(client);

        await db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> deleteClient(int id)
    {
        if (id <= 0) return false;

        Client? clientInDatabase = await db.Clients.Where(c => c.Id == id).FirstOrDefaultAsync();
        if (clientInDatabase == null) return false;

        db.Clients.Remove(clientInDatabase);

        await db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> updateClient(int idToUpdate, Client? updatedClient)
    {
        if (updatedClient == null) return false;
        if (idToUpdate <= 0 || updatedClient.Id <= 0) return false;

        Client? clientInDatabase = await db.Clients.Where(c => c.Id == idToUpdate).FirstOrDefaultAsync();
        if (clientInDatabase == null) return false;

        db.Clients.Remove(clientInDatabase);
        await db.SaveChangesAsync();

        updatedClient.UpdatedAt = DateTime.Now;

        db.Add(updatedClient);
        await db.SaveChangesAsync();

        return true;
    }

    public IEnumerable<Order>? getClientOrders(int clientId)
    {
        if (clientId <= 0) return null;

        IEnumerable<Order> orders = db.Orders.Where(o => o.BillTo == clientId || o.ShipTo == clientId);
        return orders;
    }    
}