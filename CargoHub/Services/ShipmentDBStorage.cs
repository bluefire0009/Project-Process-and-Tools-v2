using CargoHub;
using CargoHub.Models;
using Microsoft.EntityFrameworkCore;


public class ShipmnetStorage : IShipmentStorage
{
    DatabaseContext DB;

    public ShipmnetStorage(DatabaseContext db)
    {
        DB = db;
    }
    public async Task<IEnumerable<Shipment>> GetShipments()
    {
        // return all shipments
        return await DB.Shipments.ToListAsync();
    }

    public async Task<Shipment?> GetShipment(int shipmentId)
    {
        // return shipment by id
        return await DB.Shipments.FirstOrDefaultAsync(x => x.Id == shipmentId);
    }

    public async Task<IEnumerable<ShipmentItems>> GetItemsInShipment(int shipmentId)
    {
        // get all items from Shipmnet
        return await DB.ShipmentItems.Where(x => x.ShipmentId == shipmentId).ToListAsync();
    }

    public async Task<bool> AddShipment(Shipment shipment)
    {
        // add shipment to shipments
        if (shipment == null) return false;

        shipment.CreatedAt = DateTime.Now;

        await DB.Shipments.AddAsync(shipment);
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }
    public async Task<bool> UpdateShipment(int shipmentId, Shipment shipment)
    {
        // update shipment by id
        if (shipment == null) return false;

        // make sure the shipment exists
        Shipment? FoundShipment = await DB.Shipments.FirstOrDefaultAsync(x => x.Id == shipmentId);
        if (FoundShipment == null) return false;

        // make sure the id doesnt get changed
        shipment.Id = shipmentId;
        // update updated at
        shipment.UpdatedAt = DateTime.Now;

        // update exsting shipment
        DB.Shipments.Update(shipment);

        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> UpdateItemsInShipment(int shipmentId, List<ShipmentItems> list2)
    {
        // check if shipment exists
        Shipment? CurrentShipment = DB.Shipments.FirstOrDefault(x => x.Id == shipmentId);
        if (CurrentShipment == null) return false;

        // find the shipmentItems for this shipmentId
        List<ShipmentItems> list1 = DB.ShipmentItems.Where(x => x.ShipmentId == shipmentId).ToList();

        // Get items with the difference in Amount between list1 and list2, including new items
        // this gives a list of items where the amount is how to amount increased / decreased compared
        // to the corrent list of items ex: Item1 = (Id:1, Amount:-10) means that item with id 1 decreasd amount by 10
        // for example old item1 = (Id:1, Amount:20) new item1 = (Id:1, Amount: 10)
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
                    return new ShipmentItems(x.item2.ItemUid, x.item2.Amount - x.item1Group.Amount);
                }
                else
                {
                    // If item is new (not found in list1), add the entire Amount
                    return new ShipmentItems(x.item2.ItemUid, x.item2.Amount);
                }
            })
            .ToList();
        // remove all items form list where amount didnt change
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
                    inventory.total_available += item.Amount;
                    inventory.total_on_hand += item.Amount;
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
                    inventory.total_available += item.Amount;
                }
            }
            else
            {
                // if ShipmentStatus is anything else return false
                return false;
            }

        }
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }

    public async Task<bool> DelteShipment(int shipmentId)
    {
        // delete order by id
        Shipment? FoundShipment = await DB.Shipments.FirstOrDefaultAsync(x => x.Id == shipmentId);
        if (FoundShipment == null) return false;

        DB.Shipments.Remove(FoundShipment);
        if (await DB.SaveChangesAsync() < 1) return false;
        return true;
    }
}
