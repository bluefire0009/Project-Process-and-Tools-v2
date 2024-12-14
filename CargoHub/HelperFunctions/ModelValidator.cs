using CargoHub.Models;

public static class ModelValidator
{
    // takes in a Supplier to validate its fields for endpoints
    public static bool ValidateSupplier(Supplier? supplier)
    {
        if (supplier == null) return false;
        if(supplier.Code == null || supplier.Name == null || supplier.Address == null || 
            supplier.AddressExtra == null || supplier.City == null || supplier.Province == null ||
            supplier.Country == null || supplier.ContactName == null || supplier.PhoneNumber == null ||
            supplier.Reference == null) return false;
        return true;
    }
}