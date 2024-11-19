using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHub.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class DateTimeInf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLocation_Inventories_InventoryId",
                table: "InventoryLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLocation_Locations_LocationId",
                table: "InventoryLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryLocation",
                table: "InventoryLocation");

            migrationBuilder.RenameTable(
                name: "InventoryLocation",
                newName: "InventoryLocations");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryLocation_LocationId",
                table: "InventoryLocations",
                newName: "IX_InventoryLocations_LocationId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Inventories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Inventories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryLocations",
                table: "InventoryLocations",
                columns: new[] { "InventoryId", "LocationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLocations_Inventories_InventoryId",
                table: "InventoryLocations",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLocations_Locations_LocationId",
                table: "InventoryLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLocations_Inventories_InventoryId",
                table: "InventoryLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLocations_Locations_LocationId",
                table: "InventoryLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryLocations",
                table: "InventoryLocations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Inventories");

            migrationBuilder.RenameTable(
                name: "InventoryLocations",
                newName: "InventoryLocation");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryLocations_LocationId",
                table: "InventoryLocation",
                newName: "IX_InventoryLocation_LocationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryLocation",
                table: "InventoryLocation",
                columns: new[] { "InventoryId", "LocationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLocation_Inventories_InventoryId",
                table: "InventoryLocation",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLocation_Locations_LocationId",
                table: "InventoryLocation",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
