namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;

public class ItemGroup
{
    [Key]
    public int Id { get; set; }
    public required string? Name { get; set; }
    public required string? Description { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}
