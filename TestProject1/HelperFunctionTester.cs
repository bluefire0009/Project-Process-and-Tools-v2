using CargoHub.HelperFuctions;

namespace TestProject1;

[TestClass]
public class TimeZoneHelperTests
{
    [TestMethod]
    public void TestNow_ReturnsCET_CEST()
    {
        // Arrange
        DateTime utcNow = DateTime.UtcNow;

        // Act
        DateTime cetNow = CETDateTime.Now();

        // Assert
        // Calculate the time difference
        TimeSpan timeDifference = cetNow - utcNow;

        // Allow a small tolerance (1 second) for floating-point precision
        double toleranceInHours = 0.000277778;  // 1 second in hours

        // Assert that the difference is either 1 or 2 hours within the tolerance
        Assert.IsTrue(Math.Abs(timeDifference.TotalHours - 1) < toleranceInHours || Math.Abs(timeDifference.TotalHours - 2) < toleranceInHours,
            $"Expected a time difference of 1 or 2 hours, but got {timeDifference.TotalHours}.");
    }
}
