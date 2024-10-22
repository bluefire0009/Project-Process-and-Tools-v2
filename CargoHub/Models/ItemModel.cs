namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Item
{

    [Key]
    public int Uid { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public int UpcCode { get; set; }
    public string? ModelNumber { get; set; }
    public string? CommodityCode { get; set; }

    [ForeignKey("ItemLine")]
    public ItemLine? itemLine { get; set; }
    public int ItemLine { get; set; }

    [ForeignKey("ItemGroup")]
    public ItemGroup? itemGroup { get; set; }
    public int ItemGroup { get; set; }

    [ForeignKey("ItemType")]
    public ItemType? itemType { get; set; }
    public int ItemType { get; set; }

    public int UnitPurchaseQuantity { get; set; }
    public int UnitOrderQuantity { get; set; }
    public int PackOrderQuantity { get; set; }

    [ForeignKey("SupplierId")]
    public Supplier? SupplierById { get; set; }
    public int SupplierId { get; set; }

    [ForeignKey("SupplierCode")]
    public Supplier? SupplierByCode { get; set; }
    public int SupplierCode { get; set; }

    public string? SupplierPartNumber { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
}