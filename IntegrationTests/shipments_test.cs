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
public class ShipmentIntegrationTests : WebApplicationFactory<Program>
{
    private DatabaseContext _dbContext;
    private string Url = $"{Constants.VERSION}/shipments"; // Change to /shipments
    private HttpClient client;

    [TestInitialize]
    public void Setup()
    {
        var scope = Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        client = CreateClient();
        client.DefaultRequestHeaders.Add("Api-Key", Constants.API_KEY);

        addTestShipmentsToDB(client);
    }

    [TestCleanup]
    public void CleanUp()
    {
        if (_dbContext != null)
        {
            _dbContext.Database.EnsureDeleted();
        }
    }

    private void addTestShipmentsToDB(HttpClient client)
    {
        foreach (Shipment shipment in testShipments)
        {
            string jsonData = JsonConvert.SerializeObject(shipment);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            client.PostAsync($"{Url}", postContent).GetAwaiter().GetResult();
        }
    }

    private Shipment[] testShipments = [
        new Shipment() { Notes = "testshipment_1" },
        new Shipment() { Notes = "testshipment_2" }
    ];

    public int PostShipment(Shipment testShipment = null!)
    {
        if (testShipment == null)
        {
            testShipment = new Shipment() { Notes = "testshipment_3" };
        }

        string jsonData = JsonConvert.SerializeObject(testShipment);
        HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage postResponse = client.PostAsync(Url, postContent).Result;
        HttpStatusCode postStatus = postResponse.StatusCode;
        if (postStatus != HttpStatusCode.Created)
        { return -1; }

        return Convert.ToInt32(postResponse.Content.ReadAsStringAsync().Result);
    }

    public Shipment GetShipment(int shipmentId)
    {
        try
        {
            var response = client.GetAsync(Url + $"/{shipmentId}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Shipment? resultShipment = JsonConvert.DeserializeObject<Shipment>(content);
            return resultShipment!;
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public bool PutShipment(int Id, Shipment shipment)
    {
        string jsonData = JsonConvert.SerializeObject(shipment);
        HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpStatusCode putStatus = client.PutAsync(Url + $"/{Id}", putContent).Result.StatusCode;
        if (putStatus == HttpStatusCode.OK)
        { return true; }
        return false;
    }

    public bool DeleteShipment(int Id)
    {
        HttpResponseMessage deleteResponse = client.DeleteAsync(Url + $"/{Id}").Result;
        HttpStatusCode deleteStatus = deleteResponse.StatusCode;
        return deleteStatus == HttpStatusCode.OK;
    }

    [TestMethod]
    public void test_get_all_shipments()
    {
        var response = client.GetAsync(Url).Result;
        var content = response.Content.ReadAsStringAsync().Result;
        var resultShipments = JsonConvert.DeserializeObject<Shipment[]>(content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(testShipments);
        Assert.AreEqual(testShipments.Length, resultShipments!.Length);

        for (int i = 0; i < resultShipments.Length; i++)
        {
            Assert.IsTrue(testShipments[i].Equals(resultShipments[i]));
        }
    }

    [TestMethod]
    public void test_post_get_shipment()
    {
        int createdShipmentId = PostShipment();

        Shipment? resultShipment = GetShipment(createdShipmentId);

        Assert.AreNotEqual(createdShipmentId, -1);
        Assert.IsTrue(resultShipment.Id == createdShipmentId);
    }

    [TestMethod]
    public void test_put_shipment()
    {
        int createdShipmentId = PostShipment();
        Shipment? resultShipment = GetShipment(createdShipmentId);

        resultShipment.Notes = "Changed";
        Assert.IsTrue(PutShipment(createdShipmentId, resultShipment));
        Shipment updatedShipment = GetShipment(createdShipmentId);

        Assert.AreEqual(updatedShipment.Notes, "Changed");
    }

    [TestMethod]
    public void test_delete_shipment()
    {
        int createdShipmentId = PostShipment();

        Assert.IsTrue(DeleteShipment(createdShipmentId));
        Assert.IsTrue(GetShipment(createdShipmentId) == null);
    }

    [TestMethod]
    public void test_delete_shipment_with_wrong_id()
    {
        int createdShipmentId = PostShipment();

        Assert.IsFalse(DeleteShipment(-1));
    }

    [TestMethod]
    public void test_get_shipment_with_wrong_id()
    {
        int createdShipmentId = PostShipment();

        Assert.IsTrue(GetShipment(-1) == null);
    }

    [TestMethod]
    public void test_created_at()
    {
        int createdShipmentId = PostShipment();

        Shipment? resultShipment = GetShipment(createdShipmentId);

        Assert.IsNotNull(resultShipment, "The shipment should not be null after creation.");
        Assert.IsNotNull(resultShipment.CreatedAt, "The CreatedAt property should not be null.");

        var cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, cetTimeZone); // Convert UTC to CET
        DateTime createdAt = resultShipment.CreatedAt.Value;

        now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        createdAt = new DateTime(createdAt.Year, createdAt.Month, createdAt.Day, createdAt.Hour, createdAt.Minute, 0);

        Assert.AreEqual(now, createdAt, "The CreatedAt timestamp should match the current time accurate to the minute.");
    }

    [TestMethod]
    public void test_updated_at()
    {
        int createdShipmentId = PostShipment();

        Shipment? resultShipment = GetShipment(createdShipmentId);

        resultShipment.Notes = "Changed";
        Assert.IsTrue(PutShipment(createdShipmentId, resultShipment));
        Shipment updatedShipment = GetShipment(createdShipmentId);

        Assert.IsNotNull(updatedShipment, "The shipment should not be null after creation.");
        Assert.IsNotNull(updatedShipment.UpdatedAt, "The UpdatedAt property should not be null.");

        var cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, cetTimeZone); // Convert UTC to CET
        DateTime updatedAt = updatedShipment.UpdatedAt.Value;

        now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        updatedAt = new DateTime(updatedAt.Year, updatedAt.Month, updatedAt.Day, updatedAt.Hour, updatedAt.Minute, 0);

        Assert.AreEqual(now, updatedAt, "The UpdatedAt timestamp should match the current time accurate to the minute.");
    }
}
