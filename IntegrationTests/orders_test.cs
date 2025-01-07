using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using CargoHub.HelperFuctions;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using CargoHub.Config;

namespace IntegrationTests;

[ExcludeFromCodeCoverage]
[TestClass]
public class OrderIntegrationTests : WebApplicationFactory<Program>
{
    private DatabaseContext _dbContext;
    private string Url = $"{Constants.VERSION}/orders";
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

        // Add headers, including the API key
        client.DefaultRequestHeaders.Add("Api-Key", Constants.API_KEY);

        addTestOrdersToDB(client);
    }

    [TestCleanup]
    public void CleanUp()
    {
        if (_dbContext != null)
        {
            _dbContext.Database.EnsureDeleted();  // Optionally delete the database
        }
    }


    private void addTestOrdersToDB(HttpClient client)
    {
        // Add both warehouses to db
        foreach (Order order in testOrders)
        {
            string jsonData = JsonConvert.SerializeObject(order);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            client.PostAsync($"{Url}", postContent).GetAwaiter().GetResult();
        }
    }

    private Order[] testOrders = [
        new Order() {Notes = "testorder_1"},
        new Order() {Notes = "testorder_2"},
    ];

    public int PostOrder(Order testOrder = null!)
    {
        // Use a default Order object if testOrder is null
        if (testOrder == null)
        {
            testOrder = new Order()
            {
                Notes = "testorder_3"
            };
        }

        string jsonData = JsonConvert.SerializeObject(testOrder);
        HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage postResponse = client.PostAsync(Url, postContent).Result;
        HttpStatusCode postStatus = postResponse.StatusCode;
        if (postStatus != HttpStatusCode.Created)
        { return -1; }

        return Convert.ToInt32(postResponse.Content.ReadAsStringAsync().Result);
    }

    public Order GetOrder(int orderId)
    {
        try
        {
            var response = client.GetAsync(Url + $"/{orderId}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Order? resultOrder = JsonConvert.DeserializeObject<Order>(content);
            return resultOrder!;
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public bool PutOrder(int Id, Order order)
    {
        string jsonData = JsonConvert.SerializeObject(order);
        HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpStatusCode putStatus = client.PutAsync(Url + $"/{Id}", putContent).Result.StatusCode;
        if (putStatus == HttpStatusCode.OK)
        { return true; }
        return false;
    }

    public bool DeleteOrder(int Id)
    {
        HttpResponseMessage deleteResponse = client.DeleteAsync(Url + $"/{Id}").Result;
        HttpStatusCode deleteStatus = deleteResponse.StatusCode;
        return deleteStatus == HttpStatusCode.OK;
    }

    [TestMethod]
    public void test_get_all_orders()
    {
        // Tests the GET /orders endpoint
        // makes sure the user can get all the orders in the db

        // Arrange

        // Act
        var response = client.GetAsync(Url).Result;
        var content = response.Content.ReadAsStringAsync().Result;
        var resultOrders = JsonConvert.DeserializeObject<Order[]>(content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(testOrders);
        Assert.AreEqual(testOrders.Length, resultOrders!.Length);

        for (int warehouseIterator = 0; warehouseIterator < resultOrders.Length; warehouseIterator++)
        {
            Assert.IsTrue(testOrders[warehouseIterator].Equals(resultOrders[warehouseIterator]));
        }
    }

    [TestMethod]
    public void test_post_get_order()
    {
        // test the Post /locaitons endpoint
        // makes sure the user can post and retrive a location

        // Arrange
        int createdOrderId = PostOrder();

        // Act
        Order? resultOrder = GetOrder(createdOrderId);

        // Assert
        Assert.AreNotEqual(createdOrderId, -1);
        Assert.IsTrue(resultOrder.Id == createdOrderId);
    }

    [TestMethod]
    public void test_put_order()
    {
        // test the Put /orders/{id} endpoint
        // makes sure the user can modify a location in the db

        // Arrange
        int createdLOrderId = PostOrder();
        Order? resultOrder = GetOrder(createdLOrderId);

        // Act
        resultOrder.Notes = "Changed";
        Assert.IsTrue(PutOrder(createdLOrderId, resultOrder));
        Order Updatedlocation = GetOrder(createdLOrderId);

        // Assert
        Assert.AreEqual(Updatedlocation.Notes, "Changed");
    }

    [TestMethod]
    public void test_get_order_items()
    {
        // test the Get /orders/{Id}/items endpoint
        // makes sure the user can get the items from an order
        Order testOrder = new()
        {
            Items = new List<OrderItems>
                    {
                        new OrderItems(null!, 10, 1),
                        new OrderItems(null!, 20, 1),
                        new OrderItems(null!, 5, 1)
                    }
        };

        int createdLOrderId = PostOrder(testOrder);
    }

    [TestMethod]
    public void test_delete_order()
    {
        // test the Delete /locations/{id} endpoint
        // makes sure you can (soft)delete a location from the db

        // Arrange
        int createdOrderId = PostOrder();

        // Act
        // Assert
        Assert.IsTrue(DeleteOrder(createdOrderId));
        Assert.IsTrue(GetOrder(createdOrderId) == null);
    }

    [TestMethod]
    public void test_delte_order_with_wrong_id()
    {
        // test the Delete /locations/{id} endpoint
        // Makes sure the porgram handles an incorrect id propperly

        // Arrange
        int createdOrderId = PostOrder();

        // Act
        // Assert
        Assert.IsFalse(DeleteOrder(-1));
    }

    [TestMethod]
    public void test_get_with_wrong_id()
    {
        // Tests the GET /locations endpoint
        // Makes sure the porgram handles an incorrect id propperly

        // Arrange
        int createdOrderId = PostOrder();

        // Act
        // Assert
        Assert.IsTrue(GetOrder(-1) == null);
    }

    [TestMethod]
    public void test_created_at()
    {
        // Tests the Datetime format in the location
        // Makes sure its created correctly and has the correct time and format

        // Arrange
        int createdOrderId = PostOrder();

        // Act
        Order? resultorder = GetOrder(createdOrderId);

        // Assert
        Assert.IsNotNull(resultorder, "The location should not be null after creation.");

        Assert.IsNotNull(resultorder.CreatedAt, "The CreatedAt property should not be null.");

        // Get current time in CET
        var cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, cetTimeZone); // Convert UTC to CET
        DateTime createdAt = resultorder.CreatedAt.Value;

        // Round both times to the nearest minute for comparison
        now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        createdAt = new DateTime(createdAt.Year, createdAt.Month, createdAt.Day, createdAt.Hour, createdAt.Minute, 0);

        Assert.AreEqual(now, createdAt, "The CreatedAt timestamp should match the current time accurate to the minute.");

    }

    [TestMethod]
    public void test_updated_at()
    {
        // Tests the Datetime format in the location
        // Makes sure its created correctly and has the correct time and format

        // Arrange
        int createdOrderId = PostOrder();

        // Act
        Order? resultorder = GetOrder(createdOrderId);

        resultorder.Notes = "Changed";
        Assert.IsTrue(PutOrder(createdOrderId, resultorder));
        Order Updatedlocation = GetOrder(createdOrderId);

        // Assert
        Assert.IsNotNull(Updatedlocation, "The location should not be null after creation.");

        Assert.IsNotNull(Updatedlocation.UpdatedAt, "The CreatedAt property should not be null.");

        // Get current time in CET
        var cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, cetTimeZone); // Convert UTC to CET
        DateTime UpdatedAt = Updatedlocation.UpdatedAt.Value;

        // Round both times to the nearest minute for comparison
        now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, 0);

        Assert.AreEqual(now, UpdatedAt, "The UpdatedAt timestamp should match the current time accurate to the minute.");
    }
}