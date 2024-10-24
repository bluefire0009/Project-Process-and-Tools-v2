namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Shipment
{
    //     "id": 57,
    // "order_id": 57,
    // "source_id": 23,
    // "order_date": "1970-10-17",
    // "request_date": "1970-10-19",
    // "shipment_date": "1970-10-21",
    // "shipment_type": "I",
    // "shipment_status": "Delivered",
    // "notes": "Grond trouwen noch leeftijd steun zilver.",
    // "carrier_code": "DPD",
    // "carrier_description": "Dynamic Parcel Distribution",
    // "service_code": "TwoDay",
    // "payment_type": "Manual",
    // "transfer_mode": "Sea",
    // "total_package_count": 2,
    // "total_package_weight": 727.76,
    // "created_at": "1970-10-17T18:21:57Z",
    // "updated_at": "1970-10-18T20:21:57Z",
    // "items": [
    //     {
    //         "item_id": "P007042",
    //         "amount": 39
    //     },
    //     {
    //         "item_id": "P006485",
    //         "amount": 35
    //     }
    // ]

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


    

    public string? Reference { get; set; }
    public string? OrderStatus { get; set; }
    public string? Notes { get; set; }
    public string? ShippingNotes { get; set; }
    public string? PickingNotes { get; set; }

    [ForeignKey("WareHouseId")]
    public WareHouse? wareHouse { get; set; }
    public int? WareHouseId { get; set; }

    [ForeignKey("ShipTo")]
    public Location? location { get; set; }
    public int? ShipTo { get; set; }

    [ForeignKey("BillTo")]
    public Client? client { get; set; }
    public int? BillTo { get; set; }

    [ForeignKey("ShipmentId")]
    public Shipment? shipment { get; set; }
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
