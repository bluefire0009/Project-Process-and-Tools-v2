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
            builder.Services.AddScoped<IItemStorage, ItemsDBStorage>();

            builder.Services.AddDbContext<DatabaseContext>(x => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();
            app.MapControllers();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
