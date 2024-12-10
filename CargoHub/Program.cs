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

            app.Use(async (context, next) =>
            {
                // Ensure the request body can be read multiple times (e.g., for logging or processing).
                context.Request.EnableBuffering();
                using (var reader = new StreamReader(
                context.Request.Body,
                encoding: System.Text.Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true))
                {
                    string body = await reader.ReadToEndAsync();

                    // Reset the stream position to allow subsequent reads by the framework or other middleware.
                    context.Request.Body.Position = 0;
                    await System.IO.File.AppendAllTextAsync("log.txt", $"{context.Request.Path} - {body} - {context.Response.StatusCode} \n");
                }

                await next.Invoke();
            });


            app.Run();
        }
    }
}




// this is needed to make c# integration tests work
public partial class Program { }
