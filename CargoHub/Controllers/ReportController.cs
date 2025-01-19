using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using CargoHub.Models;
using Microsoft.VisualBasic;
using System.Collections;

[Route("/api/v2/report")]
// Doesn't have to be covered because we have integration tests for that
[ExcludeFromCodeCoverage]
public class ReportController : Controller
{
    private IItemStorage _itemStorage;
    private IItemGroupStorage _itemGroupStorage;
    private IItemLineStorage _itemLineStorage;
    private IItemTypeStorage _itemTypeStorage;
    private IInventoryStorage _inventoryStorage;
    private IClientStorage _clientStorage;
    private IDocksStorage _dockStorage;
    private ILocationStorage _locationStorage;
    private IOrderStorage _orderStorage;
    private IShipmentStorage _shipmentStorage;
    private ISupplierStorage _supplierStorage;
    private ITransferStorage _transferStorage;
    private IWarehouseStorage _warehouseStorage;

    public ReportController(
        IItemStorage itemStorage, 
        IItemGroupStorage itemGroupStorage, 
        IItemLineStorage itemLineStorage, 
        IItemTypeStorage itemTypeStorage, 
        IInventoryStorage inventoryStorage,
        IClientStorage clientStorage,
        IDocksStorage dockStorage,
        ILocationStorage locationStorage,
        IOrderStorage orderStorage,
        IShipmentStorage shipmentStorage,
        ISupplierStorage supplierStorage,
        ITransferStorage transferStorage,
        IWarehouseStorage warehouseStorage
    ) {
        _itemStorage = itemStorage;
        _itemGroupStorage = itemGroupStorage;
        _itemLineStorage = itemLineStorage;
        _itemTypeStorage = itemTypeStorage;
        _inventoryStorage = inventoryStorage;
        _clientStorage = clientStorage;
        _dockStorage = dockStorage;
        _locationStorage = locationStorage;
        _orderStorage = orderStorage;
        _shipmentStorage = shipmentStorage;
        _supplierStorage = supplierStorage;
        _transferStorage = transferStorage;
        _warehouseStorage = warehouseStorage;
    }

    [HttpGet("")]
    public async Task<ActionResult> TestEndpoint([FromQuery] string table, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate, [FromQuery] string? status)
    {
        // if dateStart is null set to minimal value
        DateOnly dateStart = startDate ?? DateOnly.MinValue;
        // if dateEnd is null set to max value
        DateOnly dateEnd = endDate ?? DateOnly.MaxValue;

        dynamic? data = null; // Define data outside the if/else blocks.
        data = table.ToLower() switch
        {
            "items" => await _itemStorage.GetItems(0, int.MaxValue),
            "itemgroups" => await _itemGroupStorage.getItemGroups(),
            "itemlines" => await _itemLineStorage.GetItemLines(0, int.MaxValue),
            "itemtypes" => await _itemTypeStorage.GetItemTypes(0, int.MaxValue),
            "inventories" => await _inventoryStorage.getInventories(),
            "clients" => await _clientStorage.getClients(),
            "docks" => await _dockStorage.GetAllDocksAsync(),
            "locations" => await _locationStorage.GetLocations(),
            "orders" => await _orderStorage.GetOrders(),
            "shipments" => await _shipmentStorage.GetShipments(),
            "suppliers" => await _supplierStorage.getSuppliers(),
            "transfers" => await _transferStorage.GetTransfers(),
            "warehouses" => await _warehouseStorage.getWarehouses(),
            _ => null,
        };

        if (data is null || data.Count == 0) return NoContent();

        data = status is not null && (table == "orders" || table == "transfers") 
            ? FilterListByDatesAndStatus(table, data, dateStart, dateEnd, status)
            : FilterListByDates(data, dateStart, dateEnd);
        return Ok(ListToCSVFormat(data));
    }

    private List<T> FilterListByDatesAndStatus<T>(string table, List<T> list, DateOnly startDate, DateOnly endDate, string status) {
        string statusPropertyName = table.ToLower() switch {
            "orders" => "OrderStatus",
            "transfers" => "TransferStatus",
            // "shipments" => "ShipmentStatus",
            _ => "ShipmentStatus"
        };

        return list.Where(item => {
            var property = typeof(T).GetProperty("CreatedAt");
            if (property == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not contain a property named 'CreatedAt'.");
            }

            var value = property.GetValue(item);
            if (value is DateTime createdAt)
            {
                return createdAt > startDate.ToDateTime(TimeOnly.MinValue) && createdAt < endDate.ToDateTime(TimeOnly.MaxValue);
            }
            throw new InvalidOperationException($"Property 'CreatedAt' on type {typeof(T).Name} is not a DateTime.");
        })
        .Where(item => {
            var property = typeof(T).GetProperty(statusPropertyName);
            if (property == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not contain a property named 'CreatedAt'.");
            }

            var value = property.GetValue(item);
            if (value is string itemStatus)
            {
                return itemStatus == status;
            }
            throw new InvalidOperationException($"Property '{statusPropertyName}' on type {typeof(T).Name} is not a string.");
        })
        .ToList();
    }

    private List<T> FilterListByDates<T>(List<T> list, DateOnly startDate, DateOnly endDate) {
        return list.Where(item => {
            var property = typeof(T).GetProperty("CreatedAt");
            if (property == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not contain a property named 'CreatedAt'.");
            }

            var value = property.GetValue(item);
            if (value is DateTime createdAt)
            {
                return createdAt > startDate.ToDateTime(TimeOnly.MinValue) && createdAt < endDate.ToDateTime(TimeOnly.MaxValue);
            }
            throw new InvalidOperationException($"Property 'CreatedAt' on type {typeof(T).Name} is not a DateTime.");
        })
        .ToList();
    }

    private string ListToCSVFormat<T>(List<T> list)
    {
        if (list.Count == 0) return "no entries found";
        string resultString;
        resultString = string.Join(",", list[0].GetType().GetProperties().Select(_ => _.ToString().Split(" ").Last()).ToList());
        foreach (T item in list)
        {
            resultString += "\n";
            List<string> values = new();
            foreach (PropertyInfo property in item.GetType().GetProperties())
            {
                var test = property.GetType().Namespace;
                if (property.PropertyType.Namespace == "System.Collections.Generic") {
                    values.Add($"Insert collection here");
                } else {
                    values.Add((property.GetValue(item) ?? "").ToString() ?? "");
                }
            }
            resultString += string.Join(",", values);
        }
        return resultString;
    }
}
