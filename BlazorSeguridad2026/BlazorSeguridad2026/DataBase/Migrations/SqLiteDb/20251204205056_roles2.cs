using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shares.Migrations.SqLiteDb
{
    /// <inheritdoc />
    public partial class roles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "Id", "Codigo", "Contexto", "Password", "TenantId", "UserName", "email" },
                values: new object[,]
                {
                    { 1, "0001", "SqLite", "abc 11", 0, "Usuario1", null },
                    { 2, "0002", "SqLite", "abc 22", 1, "Usuario2", null },
                    { 3, "0003", "SqLite", "abc 33", 2, "Usuario3", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
