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

namespace IntegrationTests;

[ExcludeFromCodeCoverage]
[TestClass]
public class LocationIntegrationTests : WebApplicationFactory<Program>
{
    private DatabaseContext _dbContext;
    private string LocationUrl = "/api/v2/locations";
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
            client.PostAsync($"{LocationUrl}", postContent).GetAwaiter().GetResult();
        }
    }

    private Location[] testLocations = [
        new Location() {Code = "test_code_2", Name = "Test_name_1", WareHouseId = null},
        new Location() {Code = "test_code_1", Name = "Test_name_2", WareHouseId = null},
    ];

    public int PostLocation(Location testLocation = null)
    {
        // Use a default Location object if testLocation is null
        if (testLocation == null)
        {
            testLocation = new Location() { Code = "TESTCODE", Name = "TESTNAME", WareHouseId = null };
        }

        string jsonData = JsonConvert.SerializeObject(testLocation);
        HttpContent postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage postResponse = client.PostAsync(LocationUrl, postContent).Result;
        HttpStatusCode postStatus = postResponse.StatusCode;
        if (postStatus != HttpStatusCode.Created)
        { return -1; }

        return Convert.ToInt32(postResponse.Content.ReadAsStringAsync().Result);
    }

    public Location GetLocation(int locationId)
    {
        var response = client.GetAsync(LocationUrl + $"/{locationId}").Result;
        var content = response.Content.ReadAsStringAsync().Result;
        Location? resultlocation = JsonConvert.DeserializeObject<Location>(content);
        return resultlocation!;
    }

    public bool PutLocation(int Id, Location location)
    {
        string jsonData = JsonConvert.SerializeObject(location);
        HttpContent putContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpStatusCode putStatus = client.PutAsync(LocationUrl + $"/{Id}", putContent).Result.StatusCode;
        if (putStatus == HttpStatusCode.OK)
        { return true; }
        return false;
    }

    [TestMethod]
    public void test_get_all_locations()
    {
        // Tests the GET /locations endpoint
        // makes sure the user can get all the locations in the db

        // Arrange

        // Act
        var response = client.GetAsync(LocationUrl).Result;
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

    public void test_delete_location()
    {
        // test the Delete /locations/{id} endpoint
        // makes you can (soft)delete a location from the db

        int createdLocationId = PostLocation();
    }


}
