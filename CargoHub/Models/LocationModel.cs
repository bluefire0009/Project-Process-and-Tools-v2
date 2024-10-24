namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Location
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("WareHouseId")]
    public WareHouse? wareHouse { get; set; }
    public int? WareHouseId { get; set; }

    public string? Code { get; set; }
    public string? Name { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; } = null;
}