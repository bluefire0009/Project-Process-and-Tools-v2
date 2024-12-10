namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;

public class ItemLine : IEquatable<ItemLine>
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public bool IsDeleted { get; set; } = false;

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;

    public bool Equals(ItemLine? other)
    {
        if (other is null)
            return false;

        // Compare all relevant properties except foreign keys
        return Id == other.Id &&
               Name == other.Name &&
               Description == other.Description &&
               CreatedAt == other.CreatedAt &&
               UpdatedAt == other.UpdatedAt;
    }
}
