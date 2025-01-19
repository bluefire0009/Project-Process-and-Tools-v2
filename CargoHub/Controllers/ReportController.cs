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

        // switch case to get appropriate data
        dynamic? data = table.ToLower() switch
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

        // if no data return noContent
        if (data is null || data.Count == 0) return NoContent();

        // depending on the tables filter
        data = status is not null && (table == "orders" || table == "transfers" || table == "shipments") 
            ? FilterListByDatesAndStatus(table, data, dateStart, dateEnd, status)
            : FilterListByDates(data, dateStart, dateEnd);

        // convert to CSV format
        string result = ListToCSVFormat(data);
        if (result != "")
            return Ok(result);
        else
            return NoContent();
    }

    private List<T> FilterListByDatesAndStatus<T>(string table, List<T> list, DateOnly startDate, DateOnly endDate, string status) {
        string statusPropertyName = table.ToLower() switch {
            "orders" => "OrderStatus",
            "transfers" => "TransferStatus",
            // "shipments" => "ShipmentStatus",
            _ => "ShipmentStatus"
        };

        // filter createdAt date
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
        // filter on status
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
        // filter on createdAt
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
        // if no objects return empty string
        if (list.Count == 0) return "";
        string resultString;

        // obtain all headers from object except for ones where the result is a collection
        List<string> headers = new();
        foreach (PropertyInfo property in list[0].GetType().GetProperties())
        {
            if (property.PropertyType.Namespace != "System.Collections.Generic")
                headers.Add(property.ToString().Split(" ").Last());
        }

        resultString = string.Join(",", headers);
        // for each object in the list
        foreach (T item in list)
        {
            resultString += "\n";
            List<string> values = new();
            // for each property of the object
            foreach (PropertyInfo property in item.GetType().GetProperties())
            {
                var test = property.GetType().Namespace;
                // if property value is not a collection
                if (property.PropertyType.Namespace != "System.Collections.Generic") {
                    // stringify dateTime to make readable in excel
                    if (DateTime.TryParse((property.GetValue(item)?? "").ToString(), out DateTime parsedDate)) 
                        values.Add($"\"\"\"{(property.GetValue(item) ?? "").ToString() ?? ""}\"\"\"");
                    // Add to values
                    else
                        values.Add((property.GetValue(item) ?? "").ToString() ?? "");
                }
                    
            }
            // join on result string
            resultString += string.Join(",", values);
        }
        return resultString;
    }
}
