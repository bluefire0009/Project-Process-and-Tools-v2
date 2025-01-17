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
public class LocationIntegrationTests : WebApplicationFactory<Program>
{
    private DatabaseContext _dbContext;
    private string Url = $"{Constants.VERSION}/locations";
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

        addTestLoactionsToDB(client);
    }

    [TestCleanup]
    public void CleanUp()
    {
        if (_dbContext != null)
        {
            _dbContext.Database.EnsureDeleted();  // Optionally delete the database
        }
    }

    private void addTestLoactionsToDB(HttpClient client)
    {
        // Add both warehouses to db
        foreach (Location location in testLocations)
        {
            string jsonData = JsonConvert.SerializeObject(location);
            HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            client.PostAsync($"{Url}", postContent).GetAwaiter().GetResult();
        }
    }

    private Location[] testLocations = [
        new Location() {Code = "test_code_2", Name = "Test_name_1", WareHouseId = null},
        new Location() {Code = "test_code_1", Name = "Test_name_2", WareHouseId = null},
    ];

    public int PostLocation(Location testLocation = null!)
    {
        // Use a default Location object if testLocation is null
        if (testLocation == null)
        {
            testLocation = new Location() { Code = "TESTCODE", Name = "TESTNAME", WareHouseId = null };
        }

        string jsonData = JsonConvert.SerializeObject(testLocation);
        HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage postResponse = client.PostAsync(Url, postContent).Result;
        HttpStatusCode postStatus = postResponse.StatusCode;
        if (postStatus != HttpStatusCode.Created)
        { return -1; }

        return Convert.ToInt32(postResponse.Content.ReadAsStringAsync().Result);
    }

    public Location GetLocation(int locationId)
    {
        try
        {

            var response = client.GetAsync(Url + $"/{locationId}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Location? resultlocation = JsonConvert.DeserializeObject<Location>(content);
            return resultlocation!;
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public bool PutLocation(int Id, Location location)
    {
        string jsonData = JsonConvert.SerializeObject(location);
        HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpStatusCode putStatus = client.PutAsync(Url + $"/{Id}", putContent).Result.StatusCode;
        if (putStatus == HttpStatusCode.OK)
        { return true; }
        return false;
    }

    public bool DeleteLocation(int Id)
    {
        HttpResponseMessage deleteResponse = client.DeleteAsync(Url + $"/{Id}").Result;
        HttpStatusCode deleteStatus = deleteResponse.StatusCode;
        return deleteStatus == HttpStatusCode.OK;
    }

    [TestMethod]
    public void test_get_all_locations()
    {
        // Tests the GET /locations endpoint
        // makes sure the user can get all the locations in the db

        // Arrange

        // Act
        var response = client.GetAsync(Url).Result;
        var content = response.Content.ReadAsStringAsync().Result;
        var resultLocations = JsonConvert.DeserializeObject<Location[]>(content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(testLocations);
        Assert.AreEqual(testLocations.Length, resultLocations!.Length);

        for (int warehouseIterator = 0; warehouseIterator < resultLocations.Length; warehouseIterator++)
        {
            Assert.IsTrue(testLocations[warehouseIterator].Equals(resultLocations[warehouseIterator]));
        }
    }

    [TestMethod]
    public void test_post_get_location()
    {
        // test the Post /locaitons endpoint
        // makes sure the user can post and retrive a location

        // Arrange
        int createdLocationId = PostLocation();

        // Act
        Location? resultlocation = GetLocation(createdLocationId);

        // Assert
        Assert.AreNotEqual(createdLocationId, -1);
        Assert.IsTrue(resultlocation!.Id == createdLocationId);
    }

    [TestMethod]
    public void test_put_location()
    {
        // test the Put /locations/{id} endpoint
        // makes sure the user can modify a location in the db

        // Arrange
        int createdLocationId = PostLocation();
        Location? resultlocation = GetLocation(createdLocationId);

        // Act
        resultlocation.Name = "Changed";
        Assert.IsTrue(PutLocation(createdLocationId, resultlocation));
        Location Updatedlocation = GetLocation(createdLocationId);

        // Assert
        Assert.AreEqual(Updatedlocation.Name, "Changed");
    }

    [TestMethod]
    public void test_delete_location()
    {
        // test the Delete /locations/{id} endpoint
        // makes sure you can (soft)delete a location from the db

        // Arrange
        int createdLocationId = PostLocation();

        // Act
        // Assert
        Assert.IsTrue(DeleteLocation(createdLocationId));
        Assert.IsTrue(GetLocation(createdLocationId) == null);
    }

    [TestMethod]
    public void test_delte_location_with_wrong_id()
    {
        // test the Delete /locations/{id} endpoint
        // Makes sure the porgram handles an incorrect id propperly

        // Arrange
        int createdLocationId = PostLocation();

        // Act
        // Assert
        Assert.IsFalse(DeleteLocation(-1));
    }

    [TestMethod]
    public void test_get_with_wrong_id()
    {
        // Tests the GET /locations endpoint
        // Makes sure the porgram handles an incorrect id propperly

        // Arrange
        int createdLocationId = PostLocation();

        // Act
        // Assert
        Assert.IsTrue(GetLocation(-1) == null);
    }

    [TestMethod]
    public void test_created_at()
    {
        // Tests the Datetime format in the location
        // Makes sure its created correctly and has the correct time and format

        // Arrange
        int createdLocationId = PostLocation();

        // Act
        Location? resultlocation = GetLocation(createdLocationId);

        // Assert
        Assert.IsNotNull(resultlocation, "The location should not be null after creation.");

        Assert.IsNotNull(resultlocation.CreatedAt, "The CreatedAt property should not be null.");

        // Get current time in CET
        var cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, cetTimeZone); // Convert UTC to CET
        DateTime createdAt = resultlocation.CreatedAt.Value;

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
        int createdLocationId = PostLocation();

        // Act
        Location? resultlocation = GetLocation(createdLocationId);

        resultlocation.Name = "Changed";
        Assert.IsTrue(PutLocation(createdLocationId, resultlocation));
        Location Updatedlocation = GetLocation(createdLocationId);

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
