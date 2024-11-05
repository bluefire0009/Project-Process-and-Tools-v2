namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Shipment
{

    [Key]
    public int Id { get; set; }

    [ForeignKey("OrderId")]
    public Order? order { get; set; }
    public int OrderId { get; set; }
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
}

public class ShipmentItems
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("ShipmentId")]
    public Shipment? shipment { get; set; }
    public int ShipmentId { get; set; }

    [ForeignKey("ItemUid")]
    public Item? item { get; set; }
    public int ItemUid { get; set; }

    public int Amount { get; set; }
}
