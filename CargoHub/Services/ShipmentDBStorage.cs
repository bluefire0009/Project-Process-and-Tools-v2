using CargoHub;
using CargoHub.HelperFuctions;
using CargoHub.Models;
using Microsoft.EntityFrameworkCore;


public class ShipmentStorage : IShipmentStorage
{
    DatabaseContext DB;

    public ShipmentStorage(DatabaseContext db)
    {
        DB = db;
    }
    public async Task<IEnumerable<Shipment>> GetShipments()
    {
        // Return the first 100 shipments, including their items
        return await DB.Shipments
            .Include(s => s.Items)
            .Take(100)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> GetShipments(int offset, int limit)
    {
        // Fetch shipments with pagination
        return await DB.Shipments
            .OrderBy(o => o.Id)
            .Skip(offset) // Skip the first 'offset' items
            .Take(limit)  // Take the next 'limit' items
            .ToListAsync();
    }

    public async Task<Shipment?> GetShipment(int shipmentId)
    {
        // return shipment by id
        Shipment? shipment = await DB.Shipments.FirstOrDefaultAsync(x => x.Id == shipmentId);
        List<ShipmentItems> shipmentItems = await GetItemsInShipment(shipmentId);
        // assign items to shipment
        if (shipment != null)
        { shipment.Items = shipmentItems; }
        return shipment;
    }

    public async Task<List<ShipmentItems>> GetItemsInShipment(int shipmentId)
    {
        // get all items from Shipmnet
        return await DB.ShipmentItems.Where(x => x.ShipmentId == shipmentId).ToListAsync();
    }

    public async Task<bool> AddShipment(Shipment shipment)
    {
        // add shipment to shipments
        if (shipment == null) return false;

        // extract items from order
        List<ShipmentItems> shipmentItems = shipment.Items.ToList();

        // give it the correct CreatedAt field
        shipment.CreatedAt = CETDateTime.Now();
        // add the shipment
        await DB.Shipments.AddAsync(shipment);

        // Save to make it available in the DB for the UpdateItemsInShipment
        if (await DB.SaveChangesAsync() < 1) return false;

        // update the items with add setting so it adjusts the inventories propperly
        await UpdateItemsInShipment(shipment.Id, shipmentItems, settings: "add");

        // var itms = GetItemsInOrder(order.Id);
        return true;
    }
    public async Task<bool> UpdateShipment(int shipmentId, Shipment shipment)
    {
        // update shipment by id
        if (shipment == null) return false;

        // make sure the shipment exists
        Shipment? FoundShipment = await DB.Shipments.FirstOrDefaultAsync(x => x.Id == shipmentId);
        if (FoundShipment == null) return false;

        // first empty the items incase the ShipmentStatus or Shipmenttype changed
        await UpdateItemsInShipment(shipmentId, []);

        // update orderstatus/type here as it is needed for the UpdateItemsInOrder func
        FoundShipment.ShipmentStatus = shipment.ShipmentStatus;
        FoundShipment.ShipmentType = shipment.ShipmentType;
        await DB.SaveChangesAsync();

        // Update the items first
        await UpdateItemsInShipment(shipment.Id, shipment.Items.ToList(), settings: "add");

        // update updated at
        shipment.UpdatedAt = CETDateTime.Now();

        // Update the rest of the existing shipment
        FoundShipment.SourceId = shipment.SourceId;
        FoundShipment.OrderDate = shipment.OrderDate;
        FoundShipment.RequestDate = shipment.RequestDate;
        FoundShipment.ShipmentDate = shipment.ShipmentDate;
        FoundShipment.ShipmentType = shipment.ShipmentType;
        FoundShipment.ShipmentStatus = shipment.ShipmentStatus;
        FoundShipment.Notes = shipment.Notes;
        FoundShipment.CarrierCode = shipment.CarrierCode;
        FoundShipment.CarrierDescription = shipment.CarrierDescription;
        FoundShipment.ServiceCode = shipment.ServiceCode;
        FoundShipment.PaymentType = shipment.PaymentType;
        FoundShipment.TransferMode = shipment.TransferMode;
        FoundShipment.TotalPackageCount = shipment.TotalPackageCount;
        FoundShipment.TotalPackageWeight = shipment.TotalPackageWeight;
        // FoundShipment.CreatedAt = shipment.CreatedAt;
        // FoundShipment.UpdatedAt = shipment.UpdatedAt;
        // FoundShipment.Items = shipment.Items;

        DB.OrdersInShipment.RemoveRange(FoundShipment.OrderIds);
        DB.OrdersInShipment.AddRange(shipment.OrderIds);

        await DB.SaveChangesAsync();


        // using .Update doesnt work for shipment.Items so I remove and then add the items
        foreach (var item in FoundShipment.Items)
        {
            ShipmentItems? foundItem = DB.ShipmentItems.FirstOrDefault(x => x.Id == item.Id);
            if (foundItem == null) continue;
            DB.ShipmentItems.Remove(foundItem);
        }
        await DB.SaveChangesAsync();

        foreach (var item in shipment.Items)
        {
            var existingItem = await DB.ShipmentItems.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (existingItem != null)
            {
                // Attach existing item to the context if it exists
                DB.ShipmentItems.Update(item); // Update the existing item
            }
            else
            {
                // Add new item to the context if it doesn't exist
                DB.ShipmentItems.Add(item);
            }
        }
        await DB.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateItemsInShipment(int shipmentId, List<ShipmentItems> list2, string settings = "")
    {
        // Unique Shipment Types:
        // { None, 'O', 'I'}

        // Unique Shipment Statuses:
        // { None, 'Pending', 'Transit', 'Delivered'}
        // check if shipment exists
        List<ShipmentItems> list1 = new();
        Shipment? CurrentShipment = DB.Shipments.FirstOrDefault(x => x.Id == shipmentId);
        if (CurrentShipment == null) return false;

        if (settings == "add")
        {
            list1 = new();
        }
        else
        {
            list1 = DB.ShipmentItems.Where(x => x.ShipmentId == shipmentId).ToList();
        }
        // find the shipmentItems for this shipmentId

        // Get items with the difference in Amount between list1 and list2, including new items
        // this gives a list of items where the amount is how to amount increased / decreased compared
        // to the corrent list of items ex: Item1 = (Id:1, Amount:-10) means that item with id 1 decreasd amount by 10
        // for example old item1 = (Id:1, Amount:20) new item1 = (Id:1, Amount: 10)
        // Get items from list2 with the difference in Amount from list1, including new items
        var itemsToUpdate = list2
            .GroupJoin(
                list1,
                item2 => item2.ItemUid,
                item1 => item1.ItemUid,
                (item2, item1Group) => new { item2, item1Group = item1Group.FirstOrDefault() })
            .Select(x =>
            {
                if (x.item1Group != null)
                {
                    // If item exists in list1, calculate the difference in Amount
                    return new ShipmentItems(x.item2.ItemUid, x.item2.Amount - x.item1Group.Amount, shipmentId);
                }
                else
                {
                    // If item is new (not found in list1), add the entire Amount
                    return new ShipmentItems(x.item2.ItemUid, x.item2.Amount, shipmentId);
                }
            })
            .ToList();

        // Now add items from list1 that are missing in list2 (those will have negative Amount)
        var itemsFromList1 = list1
            .Where(item1 => !list2.Any(item2 => item2.ItemUid == item1.ItemUid))
            .Select(item1 => new ShipmentItems(item1.ItemUid, -item1.Amount, shipmentId))
            .ToList();

        // Add those missing items from list1 to the update list
        itemsToUpdate.AddRange(itemsFromList1);

        // Remove items where Amount is 0
        itemsToUpdate.RemoveAll(item => item.Amount == 0);

        // get status and type of shipment and check if they are null
        string ShipmentType = CurrentShipment.ShipmentType!;
        string ShipmentStatus = CurrentShipment.ShipmentStatus!;
        if (ShipmentStatus == null || ShipmentType == null) return false;


        foreach (ShipmentItems item in itemsToUpdate)
        {
            // get inventory for the current item
            Inventory? inventory = await DB.Inventories.FirstOrDefaultAsync(x => x.ItemId == item.ItemUid);
            if (inventory == null) return false;


            if (ShipmentStatus == "Transit")
            {
                // if shipment is in transit during change and its Incomming then change total expected
                if (ShipmentType == "I")
                {
                    inventory.total_expected += item.Amount;
                }
                // if its Outgoing then change total available and total on hand
                else if (ShipmentType == "O")
                {
                    inventory.total_available -= item.Amount;
                    inventory.total_on_hand -= item.Amount;
                }
            }
            else if (ShipmentStatus == "Delivered")
            {
                // if the shipment is already delivered then change the total_on_hand and total_available
                // invert the amount if the shipment was Outgoing
                if (ShipmentType == "O")
                {
                    item.Amount *= -1;
                }
                // example item1 = (Id:1, amount:10) and Incomming means we have 10 more items than before
                // otherwise if it is Outgoing it means we have 10 less items than before
                inventory.total_available += item.Amount;
                inventory.total_on_hand += item.Amount;
            }
            else if (ShipmentStatus == "Pending")
            {
                // if the shipment is still pending and Incomming then total_expected changes
                if (ShipmentType == "I")
                {
                    inventory.total_expected += item.Amount;
                }
                // if the shipment is still pending and Outgoing then change the total_allocated and total_available
                else if (ShipmentType == "O")
                {
                    inventory.total_allocated += item.Amount;
                    inventory.total_available -= item.Amount;
                }
            }
            else
            {
                // if ShipmentStatus is anything else return false
                return false;
            }

        }
        // Update the shipment So the shipment has the updated items
        DB.Shipments.Update(CurrentShipment);
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> DeleteShipment(int shipmentId)
    {
        // delete shipment by id
        Shipment? FoundShipment = await DB.Shipments.FirstOrDefaultAsync(x => x.Id == shipmentId);
        if (FoundShipment == null) return false;

        // first remove the items from the shipment
        await UpdateItemsInShipment(FoundShipment.Id, []);

        foreach (var item in FoundShipment.Items)
        {
            item.IsDeleted = true;
        }

        foreach (var orderId in FoundShipment.OrderIds)
        {
            orderId.IsDeleted = true;
        }

        // then remove the shipment
        // DB.Shipments.Remove(FoundShipment);
        FoundShipment.IsDeleted = true;
        DB.Shipments.Update(FoundShipment);

        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }
}
