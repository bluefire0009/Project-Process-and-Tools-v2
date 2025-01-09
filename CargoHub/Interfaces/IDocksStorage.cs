

using CargoHub.Models;
public interface IDocksStorage
{
    Task<IEnumerable<Dock>> GetAllDocksAsync();
    Task<IEnumerable<Dock>> GetDocksWithPaginationAsync(int offset, int limit); // Add this line
    Task<Dock?> GetDockByIdAsync(int id);
    Task<bool> CreateDockAsync(Dock dock);
    Task<bool> SoftDeleteDockAsync(int id);
}