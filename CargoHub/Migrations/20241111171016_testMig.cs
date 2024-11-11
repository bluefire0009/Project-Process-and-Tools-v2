using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHub.Migrations
{
    /// <inheritdoc />
    public partial class testMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Warehouses_WareHouseId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_WareHouseId",
                table: "Locations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Locations_WareHouseId",
                table: "Locations",
                column: "WareHouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Warehouses_WareHouseId",
                table: "Locations",
                column: "WareHouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }
    }
}
