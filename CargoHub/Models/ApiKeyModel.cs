public class ApiKey
{
    public int Id { get; set; }
    public string Key_type { get; set; }
    public string Key_value { get; set; }  // This will store the hashed key
    public string Salt { get; set; }       // This stores the salt for the hashed key
}