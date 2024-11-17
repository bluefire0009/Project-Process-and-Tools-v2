namespace CargoHub.Models
{
    //Api model
    public class ApiKey
    {
        public int Id { get; set; }
        public string Key_type { get; set; }
        public string Key_value { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}