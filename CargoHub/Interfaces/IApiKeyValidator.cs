
namespace CargoHub.Interface
{ 
    
    public interface IApiKeyValidationInterface
    {  
        //Simple boolean 
        Task<bool> IsValidApiKeyAsync(string apiKey);
    }
}