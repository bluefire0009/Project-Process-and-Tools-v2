namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Warehouse : IEquatable<Warehouse>
{
    [Key]
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }

    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false; 

    public bool Equals(Warehouse? other)
    {
        if (other is null)
            return false;

        // Compare all relevant properties except foreign keys
        return this.Id == other.Id &&
               this.Code == other.Code &&
               this.Name == other.Name &&
               this.Address == other.Address &&
               this.Zip == other.Zip &&
               this.City == other.City &&
               this.Province == other.Province &&
               this.Country == other.Country &&
               this.ContactName == other.ContactName &&
               this.ContactPhone == other.ContactPhone &&
               this.ContactEmail == other.ContactEmail &&
               this.CreatedAt == other.CreatedAt &&
               this.UpdatedAt == other.UpdatedAt;        
    }
}