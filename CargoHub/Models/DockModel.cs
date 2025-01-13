using System.ComponentModel.DataAnnotations;

namespace CargoHub.Models;

public class Dock
{
[Key]
    public int Id { get; set; }
    public int LocationId { get; set; }
    public bool isDeleted { get; set; } = false;

 
    public ICollection<Transfer> Transfers { get; set; }


}
