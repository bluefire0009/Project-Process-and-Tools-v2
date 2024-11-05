using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHub.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class compositeKeysToInventoryLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryLocation",
                table: "InventoryLocation");

            migrationBuilder.DropIndex(
                name: "IX_InventoryLocation_InventoryId",
                table: "InventoryLocation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "InventoryLocation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryLocation",
                table: "InventoryLocation",
                columns: new[] { "InventoryId", "LocationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryLocation",
                table: "InventoryLocation");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "InventoryLocation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryLocation",
                table: "InventoryLocation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLocation_InventoryId",
                table: "InventoryLocation",
                column: "InventoryId");
        }
    }
}
