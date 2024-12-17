namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class ItemGroup
{
    [Key]
    public int Id { get; set; }
    [JsonRequired]
    public string? Name { get; set; }
    [JsonRequired]
    public string? Description { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}
