using CargoHub.Models;

public interface IShipmentStorage
{
    Task<IEnumerable<Shipment>> GetShipments();
    Task<IEnumerable<Shipment>> GetShipments(int offset, int limit, bool orderbyId = false);
    Task<Shipment?> GetShipment(int shipmentId);
    Task<List<ShipmentItems>> GetItemsInShipment(int shipmentId);
    Task<int> AddShipment(Shipment shipment);
    Task<bool> UpdateShipment(int shipmentId, Shipment shipment);
    Task<bool> UpdateItemsInShipment(int shipmentId, List<ShipmentItems> items, string settings = "", bool fromPost = false, string PrevStatus = "");
    Task<bool> DeleteShipment(int shipmentId);
}
