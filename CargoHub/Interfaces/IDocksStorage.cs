using CargoHub.Models;
public interface IDocksStorage
{
    Task<IEnumerable<Dock>> getDocks();
    Task<Dock?> getDock(int id);
    Task<bool> addDock(Dock dock);
    Task<bool> deleteDock(int id);
    Task<bool> updateDock(int id, Dock? dock);
    IEnumerable<Transfer>? getDockTransfers(int id);
}