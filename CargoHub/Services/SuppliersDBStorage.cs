using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class SupplierDBStorage : ISupplierStorage
{
    DatabaseContext db;
    public SupplierDBStorage(DatabaseContext db)
    {
        this.db = db;
    }

    public async Task<IEnumerable<Supplier>> getSuppliers()
    {
        List<Supplier> supplier = await db.Suppliers.ToListAsync();
        return supplier;
    }

    public async Task<Supplier?> getSupplier(int id)
    {
        Supplier? supplier = await db.Suppliers.Where(s => s.Id == id).FirstOrDefaultAsync();
        return supplier;
    }

    public IEnumerable<Item>? getSupplierItems(int supplierId)
    {
        if (supplierId <= 0) return null;

        IEnumerable<Item> items = db.Items.Where(l => l.SupplierId == supplierId);
        return items;
    }

    public async Task<bool> addSupplier(Supplier supplier)
    {
        if (supplier == null) return false;
        if (supplier.Id <= 0) return false;

        Supplier? supplierInDatabase = await db.Suppliers.Where(s => s.Id == supplier.Id).FirstOrDefaultAsync();
        if (supplierInDatabase != null) return false;

        await db.Suppliers.AddAsync(supplier);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> deleteSupplier(int id)
    {
        if (id <= 0) return false;

        Supplier? supplierInDatabase = await db.Suppliers.Where(s => s.Id == id).FirstOrDefaultAsync();
        if (supplierInDatabase == null) return false;

        db.Suppliers.Remove(supplierInDatabase);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> updateSupplier(int idToUpdate, Supplier? updatedSupplier)
    {
        if (updatedSupplier == null) return false;
        if (idToUpdate <= 0 || updatedSupplier.Id <= 0) return false;

        Supplier? supplierInDatabase = await db.Suppliers.Where(w => w.Id == idToUpdate).FirstOrDefaultAsync();
        if (supplierInDatabase == null) return false;

        db.Remove(supplierInDatabase);
        await db.SaveChangesAsync();

        db.Add(updatedSupplier);
        await db.SaveChangesAsync();

        return true;
    }
}