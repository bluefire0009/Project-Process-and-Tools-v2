using CargoHub.Models;
public interface ITransferStorage
{
    Task<IEnumerable<Transfer>> getTransfers();
    Task<Transfer?> getTransfer(int id);
    Task<bool> addTransfer(Transfer supplier);
    Task<bool> deleteTransfer(int id);
    Task<bool> updateTransfer(int id, Transfer? supplier);
    Task<(bool succeded, TransferDBStorage.TransferResult message)> commitTransfer(int id);
}