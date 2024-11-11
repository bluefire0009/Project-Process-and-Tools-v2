using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHub.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class updateCaseSensitiveName2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemGroups_ItemGroup",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemLines_ItemLine",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemTypes_ItemType",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemGroup",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemLine",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemType",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemGroup",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemLine",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Item_Group",
                table: "Items",
                column: "Item_Group");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Item_Line",
                table: "Items",
                column: "Item_Line");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Item_Type",
                table: "Items",
                column: "Item_Type");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemGroups_Item_Group",
                table: "Items",
                column: "Item_Group",
                principalTable: "ItemGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemLines_Item_Line",
                table: "Items",
                column: "Item_Line",
                principalTable: "ItemLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemTypes_Item_Type",
                table: "Items",
                column: "Item_Type",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemGroups_Item_Group",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemLines_Item_Line",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemTypes_Item_Type",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Item_Group",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Item_Line",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Item_Type",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "ItemGroup",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemLine",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemGroup",
                table: "Items",
                column: "ItemGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemLine",
                table: "Items",
                column: "ItemLine");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemType",
                table: "Items",
                column: "ItemType");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemGroups_ItemGroup",
                table: "Items",
                column: "ItemGroup",
                principalTable: "ItemGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemLines_ItemLine",
                table: "Items",
                column: "ItemLine",
                principalTable: "ItemLines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemTypes_ItemType",
                table: "Items",
                column: "ItemType",
                principalTable: "ItemTypes",
                principalColumn: "Id");
        }
    }
}
