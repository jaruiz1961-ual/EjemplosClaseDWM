using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataBase.Migrations.SqLiteDb
{
    /// <inheritdoc />
    public partial class Seguridad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguridad", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Seguridad",
                columns: new[] { "Id", "Password", "TenantId", "UserName", "email" },
                values: new object[,]
                {
                    { 1, "abc1", 0, "admin1", null },
                    { 2, "abc2", 0, "admin2", null },
                    { 3, "abc3", 0, "admin3", null },
                    { 4, "abc4", 1, "admin4", null },
                    { 5, "abc5", 1, "admin5", null },
                    { 6, "abc6", 1, "admin6", null },
                    { 7, "abc7", 2, "admin7", null },
                    { 8, "abc8", 2, "admin8", null },
                    { 9, "abc9", 2, "admin9", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seguridad");

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
    }
}
