

using CargoHub.Models;
public interface IDocksStorage
{
      Task<IEnumerable<Dock>> GetAllDocksAsync();
    Task<Dock?> GetDockByIdAsync(int id);
    Task<bool> CreateDockAsync(Dock dock);
    Task<bool> SoftDeleteDockAsync(int id);
}