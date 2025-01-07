using CargoHub.HelperFuctions;
using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class TransferDBStorage : ITransferStorage
{
    DatabaseContext db;
    public TransferDBStorage(DatabaseContext db)
    {
        this.db = db;
    }

    public async Task<IEnumerable<Transfer>> GetTransfers()
    {
        List<Transfer> transfers = await db.Transfers.Take(100).ToListAsync();
        return transfers;
    }

    public async Task<IEnumerable<Transfer>> GetTransfers(int offset, int limit, bool orderbyId = false)
    {
        if (orderbyId)
        {
            return await db.Transfers
                .OrderBy(o => o.Id)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
        return await db.Transfers
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
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

        // Check that transferLocations are valid
        if ((await db.Locations.FirstOrDefaultAsync(l => l.Id == transfer.TransferFrom)) == null) return false;
        if ((await db.Locations.FirstOrDefaultAsync(l => l.Id == transfer.TransferTo)) == null) return false;

        // Check if  the composite keys already exsist or if the item is null
        foreach (TransferItem item in transfer.Items)
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

        foreach (TransferItem item in transfer.Items)
        {
            await db.TransferItems.AddAsync(item);
        }
        transfer.CreatedAt = CETDateTime.Now();
        transfer.UpdatedAt = CETDateTime.Now();
        transfer.TransferStatus = "Scheduled";
        await System.IO.File.AppendAllTextAsync("log.txt", $"Scheduled batch transfer: {transfer.Id} \n");
        await db.Transfers.AddAsync(transfer);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> deleteTransfer(int id)
    {
        if (id <= 0) return false;

        Transfer? transferInDatabase = await db.Transfers.Where(s => s.Id == id).FirstOrDefaultAsync();
        if (transferInDatabase == null) return false;

        // // list of all transferItems which have to be deleted as well
        // List<TransferItem> transferItems = await db.TransferItems.Where(i => i.TransferId == id).ToListAsync();
        // foreach (TransferItem item in transferItems)
        // {
        //     db.TransferItems.Remove(item);
        // }
        transferInDatabase.IsDeleted = true;
        db.Transfers.Update(transferInDatabase);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> updateTransfer(int idToUpdate, Transfer? updatedTransfer)
    {
        if (updatedTransfer == null) return false;
        if (updatedTransfer.Id != idToUpdate) return false;
        if (idToUpdate <= 0 || updatedTransfer.Id <= 0) return false;

        // Check if  the composite keys already exsist or if the item is null
        foreach (TransferItem item in updatedTransfer.Items)
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

        foreach (TransferItem item in transferInDatabase.Items.ToList())
        {
            db.TransferItems.Remove(item);
            await db.SaveChangesAsync();
        }
        foreach (TransferItem item in updatedTransfer.Items)
        {
            await db.TransferItems.AddAsync(item);
            await db.SaveChangesAsync();
        }

        db.Remove(transferInDatabase);
        await db.SaveChangesAsync();

        updatedTransfer.UpdatedAt = CETDateTime.Now();

        db.Add(updatedTransfer);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<(bool succeded, TransferResult message)> commitTransfer(int id)
    {
        if (id <= 0) return (false, TransferResult.wrongId);

        // check if the transfer is in the database
        Transfer? transferInDatabase = await db.Transfers.Where(t => t.Id == id).FirstOrDefaultAsync();
        if (transferInDatabase == null) return (false, TransferResult.transferNotFound);

        // check if there are enough items for the transfer
        foreach (TransferItem item in transferInDatabase.Items)
            if ((await checkIfItemTransferPossible(item.ItemUid, transferInDatabase.TransferTo, item.Amount)) == false)
                return (false, TransferResult.notEnoughItems);

        // carry out the transfer
        foreach (TransferItem item in transferInDatabase.Items)
        {
            Inventory? inventoryWithAskedItem = await db.Inventories.FirstOrDefaultAsync(i => i.ItemId == item.ItemUid);
            Inventory? inventoryToTransferTo = await db.Inventories.Where(i => i.InventoryLocations.Select(l => l.InventoryId).Contains(transferInDatabase.LocationTo.Id)).FirstOrDefaultAsync();
            if (inventoryWithAskedItem == null) return (false, TransferResult.FromInventoryNotExsists);
            if (inventoryToTransferTo == null) return (false, TransferResult.ToInventoryNotExsists);

            // from calculations
            inventoryWithAskedItem.total_on_hand -= item.Amount;
            inventoryWithAskedItem.total_expected = inventoryWithAskedItem.total_on_hand + inventoryWithAskedItem.total_ordered;
            inventoryWithAskedItem.total_available = inventoryWithAskedItem.total_on_hand - inventoryWithAskedItem.total_allocated;
            // remove the item if the transfer results in total_available lower than 0
            if (inventoryWithAskedItem.total_available <= 0)
            {
                db.InventoryLocations.RemoveRange(inventoryWithAskedItem.InventoryLocations);
                db.Inventories.Remove(inventoryWithAskedItem);
                db.SaveChanges();
            }

            // to calculations
            inventoryToTransferTo.total_on_hand += item.Amount;
            inventoryToTransferTo.total_expected = inventoryToTransferTo.total_on_hand + inventoryToTransferTo.total_ordered;
            inventoryToTransferTo.total_available = inventoryToTransferTo.total_on_hand - inventoryToTransferTo.total_allocated;
            // add the inventoryLocation if it's the first
            InventoryLocation ilToAdd = new() { InventoryId = inventoryToTransferTo.Id, LocationId = transferInDatabase.TransferTo };
            if (!inventoryToTransferTo.InventoryLocations.Any(l => l.LocationId == ilToAdd.LocationId && l.InventoryId == ilToAdd.InventoryId))
            {
                inventoryToTransferTo.InventoryLocations.ToList().Add(ilToAdd);
                inventoryToTransferTo.InventoryLocations.ToArray();
            }
            db.InventoryLocations.ToList().Add(ilToAdd);
            await db.SaveChangesAsync();
        }
        
        transferInDatabase.TransferStatus = "Processed";
        await System.IO.File.AppendAllTextAsync("log.txt", $"Processed batch transfer with id: {transferInDatabase.Id} \n");
        transferInDatabase.UpdatedAt = CETDateTime.Now();
        
        await db.SaveChangesAsync();
        return (true, TransferResult.possible);
    }

    private async Task<bool> checkIfItemTransferPossible(string itemId, int locationTo, int amountToTransfer)
    {
        Inventory? inventoryWithAskedItem = await db.Inventories.FirstOrDefaultAsync(i => i.ItemId == itemId);
        Inventory? inventoryToTransferTo = await db.Inventories.Where(i => i.InventoryLocations.Select(l => l.InventoryId).Contains(locationTo)).FirstOrDefaultAsync();
        if (inventoryWithAskedItem == null) return false;
        if (inventoryToTransferTo == null) return false;

        if (inventoryWithAskedItem.total_available - amountToTransfer < 0) return false;

        return true;
    }

    public enum TransferResult
    {
        notEnoughItems,
        wrongId,
        transferNotFound,
        possible,
        ToInventoryNotExsists,
        FromInventoryNotExsists
    }
}