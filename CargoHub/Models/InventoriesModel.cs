namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("ItemId")]
    public Item item { get; set; } = null!;
    public string ItemId { get; set; }
    public string? Description { get; set; }
    public string? ItemReference { get; set; }

    public ICollection<InventoryLocation> InventoryLocations { get; set; } = new List<InventoryLocation>();
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
