using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class ClientDBTest
{
    private DatabaseContext db;

    [TestInitialize]
    public void setUp()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        db = new DatabaseContext(options);
    }

    [TestMethod]
    public void TestGetAllClients()
    {
        // Arrange
        var clients = new List<Client>
        {
            new Client { Id = 1, Name = "Client 1" },
            new Client { Id = 2, Name = "Client 2" }
        };
        db.Clients.AddRange(clients);
        db.SaveChanges();

        ClientDBStorage storage = new(db);

        // Act
        var result = storage.getClients().Result.ToList();

        // Assert
        Assert.AreEqual(clients.Count, result.Count);
        for (int i = 0; i < clients.Count; i++)
        {
            Assert.AreEqual(clients[i].Id, result[i].Id);
            Assert.AreEqual(clients[i].Name, result[i].Name);
        }
    }
    [TestMethod]
    public void TestGetClientsInPagination()
    {
        // Arrange
        var clients = new List<Client>
        {
            new Client { Id = 1, Name = "Client 1" },
            new Client { Id = 2, Name = "Client 2" },
            new Client { Id = 3, Name = "Client 3" },
            new Client { Id = 4, Name = "Client 4" }
        };
        db.Clients.AddRange(clients);
        db.SaveChanges();
        
        int offset = 1; // Start from the second client
        int limit = 2;  // Retrieve two clients

        ClientDBStorage storage = new(db);

        // Act
        var result = storage.GetClientsInPagination(offset, limit).Result.ToList();

        // Assert
        Assert.AreEqual(limit, result.Count); // Ensure the limit is respected
        Assert.AreEqual(2, result[0].Id);    // The first client in the result should match the offset
        Assert.AreEqual(3, result[1].Id);    // The second client in the result should be the next one
    }

    [TestMethod]
    [DataRow(1, true)]
    [DataRow(2, false)]
    public void TestGetClient(int clientId, bool expectedResult)
    {
        // Arrange
        db.Clients.Add(new Client { Id = 1, Name = "Client 1" });
        db.SaveChanges();
        ClientDBStorage storage = new(db);

        // Act
        var client = storage.getClient(clientId).Result;

        // Assert
        Assert.AreEqual(expectedResult, client != null);
    }

    [TestMethod]
    [DataRow(null, false)]
    [DataRow(1, true)]
    public void TestAddClient(int? clientId, bool expectedResult)
    {
        // Arrange
        var client = clientId != null ? new Client { Id = clientId.Value, Name = "Client" } : null;
        ClientDBStorage storage = new(db);

        // Act
        bool actualResult = storage.addClient(client).Result;

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void TestDeleteClient()
    {
        // Arrange
        db.Clients.Add(new Client { Id = 1, Name = "Client" });
        db.SaveChanges();
        ClientDBStorage storage = new(db);

        // Act
        bool deleteResult1 = storage.deleteClient(1).Result;
        bool deleteResult2 = storage.deleteClient(1).Result;

        // Assert
        Assert.IsTrue(deleteResult1);
        Assert.IsFalse(deleteResult2);
    }

    [TestMethod]
    public void TestUpdateClient()
    {
        // Arrange
        db.Clients.Add(new Client { Id = 1, Name = "Old Client" });
        db.SaveChanges();
        var updatedClient = new Client { Id = 1, Name = "Updated Client" };
        ClientDBStorage storage = new(db);

        // Act
        bool updateResult = storage.updateClient(1, updatedClient).Result;

        // Assert
        Assert.IsTrue(updateResult);
        Assert.AreEqual("Updated Client", db.Clients.Find(1).Name);
    }

    [TestMethod]
    public void TestGetClientOrders()
    {
        // Arrange
        var client = new Client { Id = 1, Name = "Client" };
        db.Clients.Add(client);
        db.Orders.AddRange(
            new Order { Id = 1, BillTo = 1 },
            new Order { Id = 2, ShipTo = 1 },
            new Order { Id = 3, BillTo = 2 }
        );
        db.SaveChanges();

        ClientDBStorage storage = new(db);

        // Act
        var orders = storage.getClientOrders(1).ToList();

        // Assert
        Assert.AreEqual(2, orders.Count);
        Assert.IsTrue(orders.All(o => o.BillTo == 1 || o.ShipTo == 1));
    }
}