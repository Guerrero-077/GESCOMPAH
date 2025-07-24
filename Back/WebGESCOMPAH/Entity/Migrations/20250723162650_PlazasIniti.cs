using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class PlazasIniti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "plaza",
                columns: new[] { "Id", "Active", "Capacity", "CreatedAt", "Description", "IsDeleted", "Location", "Name" },
                values: new object[,]
                {
                    { 1, true, 5000, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Espacio principal para eventos masivos", false, "Centro Ciudad", "Plaza Central" },
                    { 2, true, 3000, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zona adecuada para ferias temporales", false, "Zona Norte", "Plaza Norte" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "plaza",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "plaza",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
