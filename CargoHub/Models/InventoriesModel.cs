namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    // "id": 1,
    // "item_id": "P000001",
    // "description": "Face-to-face clear-thinking complexity",
    // "item_reference": "sjQ23408K",
    // "locations": [
    //     3211,
    //     24700,
    //     14123,
    //     19538,
    //     31071,
    //     24701,
    //     11606,
    //     11817

    [Key]
    public int Id { get; set; }

    [ForeignKey("ItemId")]
    public Item item { get; set; } = null;
    public int ItemId { get; set; }
    public string? Description { get; set; }
    public string? ItemReference { get; set; }

    public ICollection<InventoryLocation> InventoryLocations { get; set; } = new List<InventoryLocation>();
}

public class InventoryLocation
{
    [ForeignKey("InventoryId")]
    public Inventory inventory { get; set; }
    public int InventoryId { get; set; }

    [ForeignKey("LocationId")]
    public Location location { get; set; }
    public int LocationId { get; set; }
}