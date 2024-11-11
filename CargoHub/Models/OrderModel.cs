namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    [Key]
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

    public Shipment? ShipmentById { get; set; }
    public int? ShipmentId { get; set; }

    public float TotalAmount { get; set; }
    public float TotalDiscount { get; set; }
    public float TotalTax { get; set; }
    public float TotalSurcharge { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;

    public ICollection<OrderItems> Items { get; set; } = new List<OrderItems>();
}

public class OrderItems
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("OrderId")]
    public Order? order { get; set; }
    public int OrderId { get; set; }

    [ForeignKey("ItemUid")]
    public Item? item { get; set; }
    public string ItemUid { get; set; }

    public int Amount { get; set; }

    public OrderItems(string ItemUid, int amount)
    {
        this.ItemUid = ItemUid;
        this.Amount = amount;
    }
}
