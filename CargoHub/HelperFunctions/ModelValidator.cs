using CargoHub.Models;

public static class ModelValidator
{
    // takes in a Warehouse to validate its fields for endpoints
    public static bool ValidateWarehouse(Warehouse? warehouse)
    {
        if (warehouse == null) return false;
        if(warehouse.Code == null || warehouse.Name == null || warehouse.Address == null || 
            warehouse.Zip == null || warehouse.City == null || warehouse.Province == null ||
            warehouse.Country == null || warehouse.ContactEmail == null || warehouse.ContactName == null || 
            warehouse.ContactPhone == null) return false;
        return true;
    }
}