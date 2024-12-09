namespace CargoHub.Models
{
    public class ApiKey
    {
        public int Id { get; set; }
        public string Key_type { get; set; } // Type of API key (e.g., admin, manager)
        public string Key_value { get; set; } // Hashed API key
        public string Salt { get; set; } // Unique salt
     
    }
}