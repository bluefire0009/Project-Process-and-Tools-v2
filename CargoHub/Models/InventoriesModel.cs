namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("ItemId")]
    public Item item { get; set; } = null!;
    public required string? ItemId { get; set; }
    public required string? Description { get; set; }
    public required string? ItemReference { get; set; }
    public required int total_on_hand { get; set; } = 0;
    public required int total_expected { get; set; } = 0;
    public required int total_ordered { get; set; } = 0;
    public required int total_allocated { get; set; } = 0;
    public required int total_available { get; set; } = 0;

    public ICollection<InventoryLocation> InventoryLocations { get; set; } = new List<InventoryLocation>();

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}

public class InventoryLocation
{
    [ForeignKey("InventoryId")]
    public Inventory? inventory { get; set; }
    public int InventoryId { get; set; }

    [ForeignKey("LocationId")]
    public Location? location { get; set; }
    public int LocationId { get; set; }
}
