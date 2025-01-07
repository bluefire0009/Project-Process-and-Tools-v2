namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Shipment : IEquatable<Shipment>
{

    [Key]
    public int Id { get; set; }
    public ICollection<OrdersInShipment> OrderIds { get; set; } = new List<OrdersInShipment>();
    public int SourceId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime OrderDate { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime RequestDate { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime ShipmentDate { get; set; }
    public string? ShipmentType { get; set; }
    public string? ShipmentStatus { get; set; }
    public string? Notes { get; set; }
    public string? CarrierCode { get; set; }
    public string? CarrierDescription { get; set; }
    public string? ServiceCode { get; set; }
    public string? PaymentType { get; set; }
    public string? TransferMode { get; set; }
    public int TotalPackageCount { get; set; }
    public float TotalPackageWeight { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;

    public ICollection<ShipmentItems> Items { get; set; } = new List<ShipmentItems>();

    // softdelte
    public bool IsDeleted { get; set; } = false;
    public bool Equals(Shipment? other)
    {
        if (other == null) return false;

        return Id == other.Id &&
               SourceId == other.SourceId &&
               OrderDate == other.OrderDate &&
               RequestDate == other.RequestDate &&
               ShipmentDate == other.ShipmentDate &&
               ShipmentType == other.ShipmentType &&
               ShipmentStatus == other.ShipmentStatus &&
               Notes == other.Notes &&
               CarrierCode == other.CarrierCode &&
               CarrierDescription == other.CarrierDescription &&
               ServiceCode == other.ServiceCode &&
               PaymentType == other.PaymentType &&
               TransferMode == other.TransferMode &&
               TotalPackageCount == other.TotalPackageCount &&
               TotalPackageWeight == other.TotalPackageWeight &&
               Items.SequenceEqual(other.Items) &&
               (OrderIds == other.OrderIds ||
                (OrderIds != null && other.OrderIds != null && OrderIds.SequenceEqual(other.OrderIds)));
    }
}

public class ShipmentItems
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("ShipmentId")]
    public Shipment? shipment { get; set; }
    public int ShipmentId { get; set; }

    [ForeignKey("ItemUid")]
    public Item? item { get; set; }
    public string ItemUid { get; set; }

    public int Amount { get; set; }

    // softdelte
    public bool IsDeleted { get; set; } = false;

    public ShipmentItems(string ItemUid, int amount, int shipmentId)
    {
        this.ItemUid = ItemUid;
        this.Amount = amount;
        this.ShipmentId = shipmentId;
    }
}

public class OrdersInShipment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("OrderId")]
    public Order? order { get; set; }
    public int OrderId { get; set; }

    [ForeignKey("ShipmentId")]
    public Shipment? shipment { get; set; }
    public int ShipmentId { get; set; }

    // softdelte
    public bool IsDeleted { get; set; } = false;

    public OrdersInShipment(int orderId, int shipmentId)
    {
        this.ShipmentId = shipmentId;
        this.OrderId = orderId;
    }
}
