using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class TransferDBStorage : ITransferStorage
{
    DatabaseContext db;
    public TransferDBStorage(DatabaseContext db)
    {
        this.db = db;
    }

    public IEnumerable<Transfer> getTransfers()
    {
        IEnumerable<Transfer> transfer = db.Transfers.AsEnumerable();
        return transfer;
    }

    public async Task<Transfer?> getTransfer(int id)
    {
        Transfer? transfer = await db.Transfers.Where(t => t.Id == id).FirstOrDefaultAsync();
        return transfer;
    }

    public async Task<bool> addTransfer(Transfer transfer)
    {
        if (transfer == null) return false;
        if (transfer.Id <= 0) return false;

        // Check if  the composite keys already exsist or if the item is null
        foreach(TransferItem item in transfer.Items)
        {
            if (item == null) return false;
            bool containsItemUid = await db.TransferItems.Where(i => i.ItemUid == item.ItemUid).FirstOrDefaultAsync() != null;
            bool containsTransferId = await db.TransferItems.Where(i => i.TransferId == item.TransferId).FirstOrDefaultAsync() != null;
            bool containsCompositeKey = containsTransferId && containsItemUid;
            if (containsCompositeKey) return false;
            // Check if transfer holds duplicate of the composite key
            bool containsDuplicatesKeys = transfer.Items.Where(i => i.TransferId == item.TransferId && i.ItemUid == item.ItemUid).Count() > 1;
            if (containsDuplicatesKeys) return false;
            // Check if updated item actually exsists in the Items table
            bool itemExsists = db.Items.Select(i => i.Uid).Contains(item.ItemUid);
            if (!itemExsists) return false; 
            // Check if the TransferId of the item is the same as the transfer it's in
            if (item.TransferId != transfer.Id) return false;
        }

        Transfer? transferInDatabase = await db.Transfers.Where(s => s.Id == transfer.Id).FirstOrDefaultAsync(); 
        if (transferInDatabase != null) return false;

        foreach(TransferItem item in transfer.Items)
        {
            await db.TransferItems.AddAsync(item);
        }

        await db.Transfers.AddAsync(transfer);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> deleteTransfer(int id)
    {
        if (id <= 0) return false;

        Transfer? transferInDatabase = await db.Transfers.Where(s => s.Id == id).FirstOrDefaultAsync();
        if (transferInDatabase == null) return false;

        // list of all transferItems which have to be deleted as well
        List<TransferItem> transferItems = await db.TransferItems.Where(i => i.TransferId == id).ToListAsync();
        foreach (TransferItem item in transferItems)
        {
            db.TransferItems.Remove(item);
        }
        db.Transfers.Remove(transferInDatabase);
        
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> updateTransfer(int idToUpdate, Transfer? updatedTransfer)
    {
        if (updatedTransfer == null) return false;
        if (idToUpdate <= 0 || updatedTransfer.Id <= 0) return false;

        // Check if  the composite keys already exsist or if the item is null
        foreach(TransferItem item in updatedTransfer.Items)
        {
            if (item == null) return false;
            bool containsItemUid = await db.TransferItems.Where(i => i.ItemUid == item.ItemUid).FirstOrDefaultAsync() != null;
            bool containsTransferId = await db.TransferItems.Where(i => i.TransferId == item.TransferId).FirstOrDefaultAsync() != null;
            bool containsCompositeKey = containsTransferId && containsItemUid;
            if (containsCompositeKey) return false;
            // Check if transfer holds duplicate of the composite key
            bool containsDuplicatesKeys = updatedTransfer.Items.Where(i => i.TransferId == item.TransferId && i.ItemUid == item.ItemUid).Count() > 1;
            if (containsDuplicatesKeys) return false;
            // Check if updated item actually exsists in the Items table
            bool itemExsists = db.Items.Select(i => i.Uid).Contains(item.ItemUid);
            if (!itemExsists) return false; 
            // Check if the TransferId of the item is the same as the transfer it's in
            if (item.TransferId != updatedTransfer.Id) return false;
        }

        Transfer? transferInDatabase = await db.Transfers.Where(t => t.Id == idToUpdate).FirstOrDefaultAsync();
        if (transferInDatabase == null) return false;

        foreach(TransferItem item in transferInDatabase.Items.ToList())
        {
            db.TransferItems.Remove(item);
            await db.SaveChangesAsync();
        }
        foreach(TransferItem item in updatedTransfer.Items)
        {
            await db.TransferItems.AddAsync(item);
            await db.SaveChangesAsync();
        }

        db.Remove(transferInDatabase);
        await db.SaveChangesAsync();

        db.Add(updatedTransfer);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<(bool succeded, string message)> commitTransfer(int id)
    {
        if (id <= 0) return (false, "wrongId");

        Transfer? transferInDatabase = await db.Transfers.Where(t => t.Id == id).FirstOrDefaultAsync();
        if (transferInDatabase == null) return (false, "notFound");

        
        
        return (true,"");
    }

    private bool checkIfItemTransferPossible(int itemId, int locationFrom, int locationTo)
    {
        int inventoryIdWithAskedItem = 
        return true;
    }
}