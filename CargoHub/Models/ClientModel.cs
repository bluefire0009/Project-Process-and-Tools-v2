using System.ComponentModel.DataAnnotations;

namespace CargoHub.Models;

public class Client
{
    // "id": 1,
    // "name": "Raymond Inc",
    // "address": "1296 Daniel Road Apt. 349",
    // "city": "Pierceview",
    // "zip_code": "28301",
    // "province": "Colorado",
    // "country": "United States",
    // "contact_name": "Bryan Clark",
    // "contact_phone": "242.732.3483x2573",
    // "contact_email": "robertcharles@example.net",
    // "created_at": "2010-04-28 02:22:53",
    // "updated_at": "2022-02-09 20:22:35"


    [Key]
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

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
}