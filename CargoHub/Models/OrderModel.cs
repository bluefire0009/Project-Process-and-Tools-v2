namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Order : IEquatable<Order>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }
    public int SourceId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime OrderDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime RequestDate { get; set; }

    public string? Reference { get; set; }
    public string? OrderStatus { get; set; }
    public string? Notes { get; set; }
    public string? ShippingNotes { get; set; }
    public string? PickingNotes { get; set; }

    [ForeignKey("WareHouseId")]
    public Warehouse? wareHouse { get; set; }
    public int? WareHouseId { get; set; }

    [ForeignKey("ShipTo")]
    public Location? location { get; set; }
    public int? ShipTo { get; set; }

    [ForeignKey("BillTo")]
    public Client? client { get; set; }
    public int? BillTo { get; set; }

    public ICollection<ShipmentsInOrders> ShipmentIds { get; set; } = new List<ShipmentsInOrders>();

    public float TotalAmount { get; set; }
    public float TotalDiscount { get; set; }
    public float TotalTax { get; set; }
    public float TotalSurcharge { get; set; }

    [JsonIgnore]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;

    public ICollection<OrderItems> Items { get; set; } = new List<OrderItems>();

    // softdelte
    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;

    public bool Equals(Order? other)
    {
        if (other is null) return false;

        // Compare all fields
        return
               SourceId == other.SourceId &&
               OrderDate == other.OrderDate &&
               RequestDate == other.RequestDate &&
               Reference == other.Reference &&
               OrderStatus == other.OrderStatus &&
               Notes == other.Notes &&
               ShippingNotes == other.ShippingNotes &&
               PickingNotes == other.PickingNotes &&
               WareHouseId == other.WareHouseId &&
               ShipTo == other.ShipTo &&
               BillTo == other.BillTo &&
               TotalAmount == other.TotalAmount &&
               TotalDiscount == other.TotalDiscount &&
               TotalTax == other.TotalTax &&
               TotalSurcharge == other.TotalSurcharge &&
               (Items == other.Items ||
                (Items != null && other.Items != null && Items.SequenceEqual(other.Items))) &&
               (ShipmentIds == other.ShipmentIds ||
                (ShipmentIds != null && other.ShipmentIds != null && ShipmentIds.SequenceEqual(other.ShipmentIds)));
    }

    public static bool operator ==(Order? left, Order? right)
    {
        // If both are null, return true
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        // If one is null and the other isn't, return false
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        // Use the Equals method to compare the actual objects
        return left.Equals(right);
    }

    // Implement inequality operator
    public static bool operator !=(Order? left, Order? right)
    {
        return !(left == right);
    }
}

public class OrderItems : IEquatable<OrderItems>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("OrderId")]
    public Order? order { get; set; }
    public int OrderId { get; set; }

    [ForeignKey("ItemUid")]
    public Item? item { get; set; }
    public string ItemUid { get; set; }

    public int Amount { get; set; }

    // softdelte
    public bool IsDeleted { get; set; } = false;

    public OrderItems(string ItemUid, int amount, int OrderId)
    {
        this.ItemUid = ItemUid;
        this.Amount = amount;
        this.OrderId = OrderId;
    }

    public bool Equals(OrderItems? other)
    {
        if (other is null) return false;

        // Compare all fields
        return this.OrderId == other.OrderId &&
        this.ItemUid == other.ItemUid &&
        this.Amount == other.Amount;
    }


}

public class ShipmentsInOrders
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

    public ShipmentsInOrders(int orderId, int shipmentId)
    {
        this.ShipmentId = shipmentId;
        this.OrderId = orderId;
    }
}
