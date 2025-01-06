using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;



[Route("/api/v2/Test")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class TestController : Controller
{
    [HttpGet]
    public ActionResult TestEndpoint()
    {
        return Ok("1");
    }
}
