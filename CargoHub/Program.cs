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
            
            builder.Services.AddScoped<IApiKeyValidationInterface, ApiKeyValidationService>();
            builder.Services.AddScoped<IWarehouseStorage, WarehouseDBStorage>();
            builder.Services.AddScoped<ITransferStorage, TransferDBStorage>();
            builder.Services.AddScoped<ISupplierStorage, SupplierDBStorage>();
            builder.Services.AddScoped<IItemStorage, ItemsDBStorage>();
            builder.Services.AddScoped<IItemTypeStorage, ItemTypesDBStorage>();
            builder.Services.AddScoped<IItemLineStorage, ItemLinesDBStorage>();
            builder.Services.AddScoped<ILocationStorage, LocationStorage>();
            builder.Services.AddScoped<IOrderStorage, OrderStorage>();
            builder.Services.AddScoped<IShipmentStorage, ShipmentStorage>();

            builder.Services.AddDbContext<DatabaseContext>(x => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();
            app.MapControllers();

            app.MapGet("/", () => "Hello World!");

            app.Use(async (context, next) => {
                await System.IO.File.AppendAllTextAsync("log.txt", $"{context.Request.Path} - {context.Response.StatusCode} \n");
                await next.Invoke();
            });


            app.Run();
        }
    }
}




// this is needed to make c# integration tests work
public partial class Program { }
