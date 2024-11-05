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
    public DbSet<Order> Orders { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<TransferItem> TransferItems { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransferItem>().HasKey(i => new { i.TransferId, i.ItemUid });
        modelBuilder.Entity<InventoryLocation>().HasKey(l => new { l.InventoryId, l.LocationId });
        modelBuilder.Entity<Transfer>().HasOne(t => t.LocationFrom).WithMany().HasForeignKey(t => t.TransferFrom);
        modelBuilder.Entity<Transfer>().HasOne(t => t.LocationTo).WithMany().HasForeignKey(t => t.TransferTo);
    }
}