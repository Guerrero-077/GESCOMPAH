using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Establishment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Establishment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Address",
                value: "Cr 1 ");

            migrationBuilder.UpdateData(
                table: "Establishment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Address",
                value: "Cr 1 ");

            migrationBuilder.UpdateData(
                table: "Establishment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Address",
                value: "Cr 1 ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Establishment");
        }
    }
}
