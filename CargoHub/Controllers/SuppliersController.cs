using Microsoft.AspNetCore.Mvc;
using CargoHub.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

[Route("/api/v2/suppliers")]
public class SuppliersController : Controller
{
    private ISupplierStorage supplierStorage;
    public SuppliersController(ISupplierStorage supplierStorage)
    {
        this.supplierStorage = supplierStorage;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetSuppliers([FromQuery] int offset = 0, [FromQuery] int limit = 100, [FromQuery] bool orderById = false)
    {
        if (offset < 0) return BadRequest("offset cannot be less than 0");
        IEnumerable<Supplier> suppliers = await supplierStorage.getSuppliers(offset, limit, orderById);
        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificSupplier(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Supplier? foundSupplier = await supplierStorage.getSupplier(id);
        if (foundSupplier == null) return NotFound($"No supplier with id:{id} found");

        return Ok(foundSupplier);
    }
    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItemsSpecificSupplier(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");

        Supplier? foundSupplier = await supplierStorage.getSupplier(id);
        if (foundSupplier == null) return NotFound($"No supplier with id:{id} found");

        List<Item> supplierItem = supplierStorage.getSupplierItems(id).ToList();

        return Ok(supplierItem);
    }

    [HttpPost("")]
    public async Task<IActionResult> PostSupplier([FromBody] Supplier supplier)
    {
        bool added = await supplierStorage.addSupplier(supplier);

        if (!added) return BadRequest($"Couldn't add supplier:{JsonConvert.SerializeObject(supplier)}");
        return Created("",$"Added supplier:{JsonConvert.SerializeObject(supplier)}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveSupplier(int id)
    {
        if (id <= 0) return BadRequest("Invalid id in the url");
        bool deleted = await supplierStorage.deleteSupplier(id);

        if (!deleted) return NotFound($"No supplier with id:{id} in the database");
        return Ok($"Deleted supplier with id: {id}");
    }

    [HttpPut("{idToUpdate}")]
    public async Task<IActionResult> UpdateSupplier(int idToUpdate, [FromBody] Supplier updatedSupplier)
    {
        if (idToUpdate <= 0) return BadRequest("Invalid id in the url");
        if (updatedSupplier == null) BadRequest("updatedSupplier cannot be null");
        if (!ModelValidator.ValidateSupplier(updatedSupplier)) return BadRequest("updatedSupplier cannot have invalid fields");

        bool updated = await supplierStorage.updateSupplier(idToUpdate, updatedSupplier);

        if (!updated) return NotFound($"No supplier with id:{idToUpdate} in the database");
        return Ok($"Updated supplier id:{idToUpdate} to:{updatedSupplier}");
    }
}