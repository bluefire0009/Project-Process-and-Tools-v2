namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Item : IEquatable<Item>
{

    [Key]
    public string Uid { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public int UpcCode { get; set; }
    public string? ModelNumber { get; set; }
    public string? CommodityCode { get; set; }

    [ForeignKey("Item_Line")]
    public ItemLine? itemLine { get; set; }
    public int Item_Line { get; set; }

    [ForeignKey("Item_Group")]
    public ItemGroup? itemGroup { get; set; }
    public int Item_Group { get; set; }

    [ForeignKey("Item_Type")]
    public ItemType? itemType { get; set; }
    public int Item_Type { get; set; }

    public int UnitPurchaseQuantity { get; set; }
    public int UnitOrderQuantity { get; set; }
    public int PackOrderQuantity { get; set; }

    [ForeignKey("SupplierId")]
    public Supplier? SupplierById { get; set; }
    public int SupplierId { get; set; }
    public int SupplierCode { get; set; }
    public string? SupplierPartNumber { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;

    public bool Equals(Item? other)
    {
        if (other is null)
            return false;

        // Compare all relevant properties except foreign keys
        return Uid == other.Uid &&
               Code == other.Code &&
               Description == other.Description &&
               ShortDescription == other.ShortDescription &&
               UpcCode == other.UpcCode &&
               ModelNumber == other.ModelNumber &&
               CommodityCode == other.CommodityCode &&
               UnitPurchaseQuantity == other.UnitPurchaseQuantity &&
               UnitOrderQuantity == other.UnitOrderQuantity &&
               PackOrderQuantity == other.PackOrderQuantity &&
               SupplierCode == other.SupplierCode &&
               SupplierPartNumber == other.SupplierPartNumber &&
               CreatedAt == other.CreatedAt &&
               UpdatedAt == other.UpdatedAt;
    }
}