using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace CargoHub
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IWarehouseStorage, WarehouseDBStorage>();
            builder.Services.AddScoped<ITransferStorage, TransferDBStorage>();
            builder.Services.AddScoped<ISupplierStorage, SupplierDBStorage>();
            builder.Services.AddScoped<IItemStorage, ItemsDBStorage>();
            builder.Services.AddScoped<IItemTypeStorage, ItemTypesDBStorage>();
            builder.Services.AddScoped<IItemLineStorage, ItemLinesDBStorage>();
            builder.Services.AddScoped<ILocationStorage, LocationStroage>();
            builder.Services.AddScoped<IOrderStorage, OrderStroage>();
            builder.Services.AddScoped<IShipmentStorage, ShipmentStorage>();

            builder.Services.AddDbContext<DatabaseContext>(x => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();
            app.MapControllers();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}




// this is needed to make c# integration tests work
public partial class Program { }
