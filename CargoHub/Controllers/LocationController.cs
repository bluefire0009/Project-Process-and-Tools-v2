using System.Diagnostics.CodeAnalysis;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc;


[Route("/api/v2/locations")]
[ExcludeFromCodeCoverage]
public class LocationController : Controller
{
    private ILocationStorage Storage;

    public LocationController(ILocationStorage storage)
    {
        Storage = storage;
    }

    [HttpGet("")]
    public
}