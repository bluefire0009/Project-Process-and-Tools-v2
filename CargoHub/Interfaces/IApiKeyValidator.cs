

    
    public interface IApiKeyValidationInterface
    {  
        //Simple boolean 
        Task<bool> IsValidAdminApiKeyAsync(string apiKey);
    }
