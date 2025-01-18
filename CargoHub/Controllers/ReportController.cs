using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CargoHub.Models;
using Microsoft.AspNetCore.Mvc;

[Route("/api/v2/Test")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class ReportController : Controller
{
    private IItemStorage _itemStorage;
    private IItemGroupStorage _itemGroupStorage;

    public ReportController(IItemStorage itemStorage, IItemGroupStorage itemGroupStorage) {
        _itemStorage = itemStorage;
        _itemGroupStorage = itemGroupStorage;
    }

    [HttpGet]
    public async Task<ActionResult> TestEndpoint<T>([FromQuery] string table, [FromQuery] DateOnly? dateStart, [FromQuery] DateOnly? dateEnd)
    {
        dynamic? data = null; // Define data outside the if/else blocks.

        switch (table.ToLower())
        {
            case "items":
                data = await _itemStorage.GetItems(0, int.MaxValue);
                break;
            case "itemgroups":
                data = await _itemGroupStorage.getItemGroups();
                break;
            default:
                data = null;
                break;
        }

        string loweredTable = table.ToLower();
        if (loweredTable == "items") {
            data = await _itemStorage.GetItems(0, int.MaxValue);
        } else if (loweredTable == "itemgroups") {
            // data = await _itemGroupStorage.GetItems(0, int.MaxValue);
        } else {
            data = null; // Optional, as `data` is already null by default.
        }

        return Ok(ListToCSVFormat(data));
    }

    private string ListToCSVFormat<T>(List<T> list)
    {
        return list.ToString();
    }
}
