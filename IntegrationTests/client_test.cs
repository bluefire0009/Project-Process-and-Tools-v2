using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ClientIntegrationTests : WebApplicationFactory<Program>
    {
        private DatabaseContext _dbContext;
        private string ClientsUrl = "/api/v2/clients";
        private string OrdersUrl = "/api/v2/orders";
        private Client[] testClients = [
                new Client(){Id = 1, Name = "Milan", Address = "Jasmijnstraat 53", City = "Papendrecht", ZipCode = "3353 CG", Province = "Zuid-Holland", Country = "Nederland", ContactName = "Milan Versluis", ContactPhone = "0638182257", ContactEmail = "milan22versluis@gmail.com"},
                new Client(){Id = 2, Name = "Bram", Address = "Jasmijnstraat 153", City = "Papendrecht", ZipCode = "9000 CG", Province = "Zuid-Holland", Country = "Nederland", ContactName = "Bram Nederlof", ContactPhone = "06123456789", ContactEmail = "Bram22nederlof@gmail.com"},
            ];
        private HttpClient client;

        [TestInitialize]
        public void Setup()
        {
            // Get the service provider and create a new scope for each test
            var scope = Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            // Reset the database to ensure it is clean for each test
            _dbContext.Database.EnsureDeleted();  // Delete any existing database
            _dbContext.Database.EnsureCreated();  // Create a new fresh database
            client = CreateClient();
            addTestClientsToDB(client);
        }
    }
     private void addTestClientsToDB(HttpClient client)
        {
            // Add both Clients to db
            foreach(Client client in testClients)
            {
                string jsonData = JsonConvert.SerializeObject(client);
                HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.PostAsync($"{ClientsUrl}", postContent).GetAwaiter().GetResult();
            }
    
    }
    [TestCleanup]
        public void CleanUp()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();  // Optionally delete the database
            }
        }
}
        