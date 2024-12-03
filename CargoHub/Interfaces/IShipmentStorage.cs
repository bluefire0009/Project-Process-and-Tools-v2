using CargoHub.Models;

public interface IShipmentStorage
{
    Task<IEnumerable<Shipment>> GetShipments();
    Task<IEnumerable<Shipment>> GetShipments(int offset, int limit);
    Task<Shipment?> GetShipment(int shipmentId);
    Task<List<ShipmentItems>> GetItemsInShipment(int shipmentId);
    Task<bool> AddShipment(Shipment shipment);
    Task<bool> UpdateShipment(int shipmentId, Shipment shipment);
    Task<bool> UpdateItemsInShipment(int shipmentId, List<ShipmentItems> items, string settings = "");
    Task<bool> DelteShipment(int shipmentId);
}
