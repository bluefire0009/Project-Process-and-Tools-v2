using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHub.Migrations
{
    /// <inheritdoc />
    public partial class m3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Items_ItemId",
                table: "Inventories");

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

            migrationBuilder.AlterColumn<string>(
                name: "ItemUid",
                table: "TransferItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ItemUid",
                table: "ShipmentItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ItemUid",
                table: "OrderItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Uid",
                table: "Items",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "Inventories",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryLocations",
                table: "InventoryLocations",
                columns: new[] { "InventoryId", "LocationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Items_ItemId",
                table: "Inventories",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Uid");

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
                name: "FK_Inventories_Items_ItemId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLocations_Inventories_InventoryId",
                table: "InventoryLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLocations_Locations_LocationId",
                table: "InventoryLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryLocations",
                table: "InventoryLocations");

            migrationBuilder.RenameTable(
                name: "InventoryLocations",
                newName: "InventoryLocation");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryLocations_LocationId",
                table: "InventoryLocation",
                newName: "IX_InventoryLocation_LocationId");

            migrationBuilder.AlterColumn<int>(
                name: "ItemUid",
                table: "TransferItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ItemUid",
                table: "ShipmentItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ItemUid",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Uid",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Inventories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryLocation",
                table: "InventoryLocation",
                columns: new[] { "InventoryId", "LocationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Items_ItemId",
                table: "Inventories",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Cascade);

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
