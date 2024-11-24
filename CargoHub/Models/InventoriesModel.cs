namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("ItemId")]
    public Item item { get; set; } = null!;
    public string? ItemId { get; set; }
    public string? Description { get; set; }
    public string? ItemReference { get; set; }
    public int total_on_hand { get; set; } = 0;
    public int total_expected { get; set; } = 0;
    public int total_ordered { get; set; } = 0;
    public int total_allocated { get; set; } = 0;
    public int total_available { get; set; } = 0;

    public ICollection<InventoryLocation> InventoryLocations { get; set; } = new List<InventoryLocation>();

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
}

public class InventoryLocation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("InventoryId")]
    public Inventory? inventory { get; set; }
    public int InventoryId { get; set; }

    [ForeignKey("LocationId")]
    public Location? location { get; set; }
    public int LocationId { get; set; }
}
