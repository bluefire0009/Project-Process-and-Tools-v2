namespace CargoHub.HelperFuctions;
public static class CETDateTime
{
    private static readonly TimeZoneInfo cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    // Replaces DateTime.Now() with CET/CEST
    public static DateTime Now()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cetTimeZone);
    }
}
