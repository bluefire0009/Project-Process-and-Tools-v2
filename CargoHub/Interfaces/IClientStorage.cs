using CargoHub.Models;
public interface IClientStorage
{
    Task<IEnumerable<Client>> getClients();
    Task<IEnumerable<Client>> GetClientsInPagination(int offset, int limit);
    Task<Client?> getClient(int id);
    Task<bool> addClient(Client client);
    Task<bool> deleteClient(int id);
    Task<bool> updateClient(int id, Client? client);
    IEnumerable<Order>? getClientOrders(int id);
}