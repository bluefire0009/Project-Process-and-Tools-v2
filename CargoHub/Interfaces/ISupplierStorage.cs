using CargoHub.Models;
public interface ISupplierStorage
{
    Task<IEnumerable<Supplier>> getSuppliers();
    Task<Supplier?> getSupplier(int id);
    Task<bool> addSupplier(Supplier supplier);
    Task<bool> deleteSupplier(int id);
    Task<bool> updateSupplier(int id, Supplier? supplier);
    IEnumerable<Item>? getSupplierItems(int id);
}