
using Microsoft.EntityFrameworkCore;

using CargoHub.Interface;

using System.Diagnostics.CodeAnalysis;

namespace CargoHub.Services
{
    [ExcludeFromCodeCoverage] //Method gets called in the filter. Filter gets tested.
    public class ApiKeyValidationService : IApiKeyValidationInterface
    {
        private readonly DatabaseContext _context;

        
        public ApiKeyValidationService(DatabaseContext context)
        {
            _context = context;
        }

        // Method to validate 
        public async Task<bool> IsValidAdminApiKeyAsync(string apiKey)
        {
            // Check value and if is admin
            var exists = await _context.ApiKeys
                                       .Where(k => k.Key_value == apiKey && k.Key_type == "admin")
                                       .AnyAsync();

            return exists;  // True exist and == admin
        }
    }
}