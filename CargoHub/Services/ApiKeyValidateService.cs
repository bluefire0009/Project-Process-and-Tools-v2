using CargoHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class ApiKeyValidationService : IApiKeyValidationInterface
{
    private readonly DatabaseContext _context;

    public ApiKeyValidationService(DatabaseContext context)
    {
        _context = context;
    }

    // Hashing method using SHA-256
    public static string HashKeyWithSalt(string key, string salt)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string combined = key + salt;  // Combine the key and salt before hashing
            byte[] bytes = Encoding.UTF8.GetBytes(combined);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);  // Convert the hash to a base64 string for storage
        }
    }

    // Method to verify the key by comparing the hash
    public static bool VerifyKey(string inputKey, string storedHash, string salt)
    {
        // Hash the input key with the stored salt
        var hashedInput = HashKeyWithSalt(inputKey, salt);
        return hashedInput == storedHash;
    }

    // Asynchronous method to validate the API key
    public async Task<bool> IsValidApiKeyAsync(string apiKey)
    {
        // List of valid key types (can add more if needed)
        List<string> validKeyTypes = new List<string> { "manager", "admin" };

        // Fetch the API keys from the database where the Key_type is valid and asynchronously get them
        var apiKeyRecords = await _context.ApiKeys
            .Where(k => validKeyTypes.Contains(k.Key_type))  // Filter by valid key types
            .ToListAsync();  // Execute the query to fetch the data asynchronously

        // Now verify the key on the client-side using the VerifyKey method
        var validKey = apiKeyRecords
            .FirstOrDefault(k => VerifyKey(apiKey, k.Key_value, k.Salt)) != null;

        return validKey;  // Return true if the key was valid, false otherwise
    }
}
