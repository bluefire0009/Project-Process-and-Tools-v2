namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Inventory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("ItemId")]
    public Item item { get; set; } = null!;
    [JsonRequired]
    public string? ItemId { get; set; }
    [JsonRequired]
    public string? Description { get; set; }
    [JsonRequired]
    public string? ItemReference { get; set; }
    [JsonRequired]
    public int total_on_hand { get; set; } = 0;
    [JsonRequired]
    public int total_expected { get; set; } = 0;
    [JsonRequired]
    public int total_ordered { get; set; } = 0;
    [JsonRequired]
    public int total_allocated { get; set; } = 0;
    [JsonRequired]
    public int total_available { get; set; } = 0;

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
