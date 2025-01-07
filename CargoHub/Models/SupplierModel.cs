namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Supplier : IEquatable<Supplier>
{
    [Key]
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? AddressExtra { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? ContactName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Reference { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }

    public bool Equals(Supplier? other)
    {
        if (other is null)
            return false;

        // Compare all relevant properties except foreign keys
        return this.Id == other.Id &&
               this.Code == other.Code &&
               this.Name == other.Name &&
               this.Address == other.Address &&
               this.AddressExtra == other.AddressExtra &&
               this.City == other.City &&
               this.ZipCode == other.ZipCode &&
               this.Province == other.Province &&
               this.Country == other.Country &&
               this.ContactName == other.ContactName &&
               this.PhoneNumber == other.PhoneNumber &&
               this.Reference == other.Reference &&
               this.CreatedAt == other.CreatedAt &&
               this.UpdatedAt == other.UpdatedAt;                    
    }
}