namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Location : IEquatable<Location>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("WareHouseId")]
    public Warehouse? wareHouse { get; set; }
    public int? WareHouseId { get; set; }

    public string? Code { get; set; }
    public string? Name { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; } = null;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;

    // softdelte
    public bool IsDeleted { get; set; } = false;

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

    public static bool operator ==(Location? left, Location? right)
    {
        // If both are null, return true
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        // If one is null and the other isn't, return false
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        // Use the Equals method to compare the actual objects
        return left.Equals(right);
    }

    // Implement inequality operator
    public static bool operator !=(Location? left, Location? right)
    {
        return !(left == right);
    }
}