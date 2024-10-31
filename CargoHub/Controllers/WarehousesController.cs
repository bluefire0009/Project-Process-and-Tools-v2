using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;

[Route("/api/v2/warehouses")]
public class WarehousesController : Controller
{
    private IWarehouseStorage warehouseStorage;
    public WarehousesController(IWarehouseStorage warehouseStorage)
    {
        this.warehouseStorage = warehouseStorage;
    }
    [HttpGet("")]
    public async Task<IActionResult> GetAllWarehouses()
    {
        List<Warehouse> warehouses = warehouseStorage.getWarehouses().ToList();
        return Ok(warehouses);
    }
}