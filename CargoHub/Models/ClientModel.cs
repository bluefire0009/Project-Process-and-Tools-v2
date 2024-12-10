using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CargoHub.Models;

public class Client
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public string Province { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string ContactName { get; set; } = string.Empty;

    public string ContactPhone { get; set; } = string.Empty;

    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;

    [JsonIgnore]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; } = null;

    [JsonIgnore]
    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
}
