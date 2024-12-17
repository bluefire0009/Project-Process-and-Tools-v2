using System.ComponentModel.DataAnnotations;

namespace CargoHub.Models;

public class Client
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; } = string.Empty;

    public required string Address { get; set; } = string.Empty;

    public required string City { get; set; } = string.Empty;

    public required string ZipCode { get; set; } = string.Empty;

    public required string Province { get; set; } = string.Empty;

    public required string Country { get; set; } = string.Empty;

    public required string ContactName { get; set; } = string.Empty;

    public required string ContactPhone { get; set; } = string.Empty;

    [EmailAddress]
    public required string ContactEmail { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}
