using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CargoHub.Models;

public class Client
{
    [Key]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; } = string.Empty;

    [JsonRequired]
    public string Address { get; set; } = string.Empty;

    [JsonRequired]
    public string City { get; set; } = string.Empty;

    [JsonRequired]
    public string ZipCode { get; set; } = string.Empty;

    [JsonRequired]
    public string Province { get; set; } = string.Empty;

    [JsonRequired]
    public string Country { get; set; } = string.Empty;

    [JsonRequired]
    public string ContactName { get; set; } = string.Empty;

    [JsonRequired]
    public string ContactPhone { get; set; } = string.Empty;

    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}
