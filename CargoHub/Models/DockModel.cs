using System.ComponentModel.DataAnnotations;

namespace CargoHub.Models;

public class Dock
{
    public int Id {get;set;}

    public int LocationId {get;set;}
    
    public int TransferId {get;set;}
    public bool isDeleted {get;set;} = false;

    

}
