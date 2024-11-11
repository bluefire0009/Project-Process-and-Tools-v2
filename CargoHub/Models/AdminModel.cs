using System.ComponentModel.DataAnnotations;

namespace CargoHub.Models;

public class Admin
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } 
    public string Password {get;set;}

    public string Status {get;set;}

}
