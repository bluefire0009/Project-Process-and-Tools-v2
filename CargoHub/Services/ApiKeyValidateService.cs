using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage] // Method gets called in the filter. Filter gets tested.
public class ApiKeyValidationService : IApiKeyValidationInterface
{
    private readonly DatabaseContext _context;

    public ApiKeyValidationService(DatabaseContext context)
    {
        _context = context;
    }

    // Hash a key with a provided salt
    public static string HashKeyWithSalt(string key, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var combined = key + salt; // Combine key and salt
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    // Method to validate the API key
    public async Task<bool> IsValidApiKeyAsync(string apiKey)
    {
        // List of valid API key types
        List<string> validKeyTypes = new List<string> { "floor_manager", "warehouse_manager", "admin", "user" };

        // Retrieve all API keys with valid types from the database
        var apiKeyRecords = await _context.ApiKeys
                                          .Where(k => validKeyTypes.Contains(k.Key_type))
                                          .ToListAsync();

        // Validate the provided API key against all records
        foreach (var record in apiKeyRecords)
        {
            // Use the stored salt and hash to verify the API key
            var hashedInput = HashKeyWithSalt(apiKey, record.Salt);
            if (hashedInput == record.Key_value)
            {
                return true; // Valid key found
            }
        }

        return false; // No valid key matches
    }
}