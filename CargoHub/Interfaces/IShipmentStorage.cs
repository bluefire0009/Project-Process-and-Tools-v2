using CargoHub.Models;

public interface IShipmentStorage
{
    Task<IEnumerable<Shipment>> GetShipments();
    Task<Shipment?> GetShipment(int shipmentId);
    Task<IEnumerable<ShipmentItems>> GetItemsInShipment(int shipmentId);
    Task<bool> AddShipment(Shipment shipment);
    Task<bool> UpdateShipment(int shipmentId, Shipment shipment);
    Task<bool> UpdateItemsInShipment(int shipmentId, List<ShipmentItems> items);
    Task<bool> DelteShipment(int shipmentId);
}
