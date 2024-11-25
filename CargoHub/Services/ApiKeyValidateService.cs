
using Microsoft.EntityFrameworkCore;



using System.Diagnostics.CodeAnalysis;


    [ExcludeFromCodeCoverage] //Method gets called in the filter. Filter gets tested.
    public class ApiKeyValidationService : IApiKeyValidationInterface
    {
        private readonly DatabaseContext _context;

        
        public ApiKeyValidationService(DatabaseContext context)
        {
            _context = context;
        }

        // Method to validate 
       
        public async Task<bool> IsValidApiKeyAsync(string apiKey)
        {
            // List of valid API key types
            List<string> validKeyTypes = new List<string> { "admin" };

            // Check if the key exists in the database with a valid key type
            var exists = await _context.ApiKeys
                                       .Where(k => k.Key_value == apiKey && validKeyTypes.Contains(k.Key_type))
                                       .AnyAsync();

            return exists; // True if it exists with a valid type
        }
    }
