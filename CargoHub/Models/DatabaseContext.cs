using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<ItemGroup> ItemGroups { get; set; }
    public DbSet<ItemLine> ItemLines { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemType> ItemTypes { get; set; }
    public DbSet<Location> Locations { get; set; }


    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<Item>()
    //         .HasOne(i => i.SupplierById)
    //         .WithMany() // Assuming no collection in Supplier
    //         .HasForeignKey(i => i.SupplierId)
    //         .HasPrincipalKey(s => s.SupplierPrimaryKey); // Maps SupplierId to SupplierPrimaryKey

    //     modelBuilder.Entity<Item>()
    //         .HasOne(i => i.SupplierByCode)
    //         .WithMany() // Assuming no collection in Supplier
    //         .HasForeignKey(i => i.SupplierCode)
    //         .HasPrincipalKey(s => s.SupplierUniqueCode); // Maps SupplierCode to SupplierUniqueCode
    // }

}