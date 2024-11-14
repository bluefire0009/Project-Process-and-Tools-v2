using System.Threading.Tasks;

namespace CargoHub.Interface
{
    public interface IApiKeyValidationInterface
    {  
        //Simple boolean 
        Task<bool> IsValidAdminApiKeyAsync(string apiKey);
    }
}