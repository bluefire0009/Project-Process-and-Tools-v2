using CargoHub.Models;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : DbContext
{
        public DbSet<Client> Clients { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryLocation> InventoryLocations { get; set; }
        public DbSet<ItemGroup> ItemGroups { get; set; }
        public DbSet<ItemLine> ItemLines { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<ShipmentsInOrders> ShipmentsInOrders { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentItems> ShipmentItems { get; set; }
        public DbSet<OrdersInShipment> OrdersInShipment { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferItem> TransferItems { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<Supplier>().HasQueryFilter(s => !s.IsDeleted);
                modelBuilder.Entity<Warehouse>().HasQueryFilter(w => !w.IsDeleted);
                modelBuilder.Entity<Item>().HasQueryFilter(i => !i.IsDeleted);
                modelBuilder.Entity<ItemType>().HasQueryFilter(i => !i.IsDeleted);
                modelBuilder.Entity<Location>().HasQueryFilter(l => !l.IsDeleted);
                modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
                modelBuilder.Entity<Shipment>().HasQueryFilter(s => !s.IsDeleted);
                modelBuilder.Entity<OrdersInShipment>().HasQueryFilter(s => !s.IsDeleted);
                modelBuilder.Entity<ShipmentsInOrders>().HasQueryFilter(s => !s.IsDeleted);
                modelBuilder.Entity<OrderItems>().HasQueryFilter(s => !s.IsDeleted);
                modelBuilder.Entity<ShipmentItems>().HasQueryFilter(s => !s.IsDeleted);
                modelBuilder.Entity<ItemLine>().HasQueryFilter(i => !i.IsDeleted);

                modelBuilder.Entity<ApiKey>().ToTable("API_keys");
                modelBuilder.Entity<TransferItem>().HasKey(i => new { i.TransferId, i.ItemUid });
                modelBuilder.Entity<InventoryLocation>().HasKey(l => new { l.InventoryId, l.LocationId });
                modelBuilder.Entity<Transfer>().HasOne(t => t.LocationFrom).WithMany().HasForeignKey(t => t.TransferFrom);
                modelBuilder.Entity<Transfer>().HasOne(t => t.LocationTo).WithMany().HasForeignKey(t => t.TransferTo);
        }
}