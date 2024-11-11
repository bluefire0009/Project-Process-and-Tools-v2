namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Location : IEquatable<Location>
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("WareHouseId")]
    private Warehouse? wareHouse { get; set; } = null;
    public int? WareHouseId { get; set; }

    public string? Code { get; set; }
    public string? Name { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;

    public bool Equals(Location? other)
    {
        // If the other object is null, return false
        if (other is null) return false;

        // Compare properties
        return Id == other.Id &&
               WareHouseId == other.WareHouseId &&
               Code == other.Code &&
               Name == other.Name &&
               CreatedAt == other.CreatedAt &&
               UpdatedAt == other.UpdatedAt;
    }
}