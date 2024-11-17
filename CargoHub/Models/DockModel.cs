using System.ComponentModel.DataAnnotations;

namespace CargoHub.Models;

public class Dock
{
    public int Id { get; set; }
    public string ZipCode { get; set; }
    public int TransferID { get; set; }  
   
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
