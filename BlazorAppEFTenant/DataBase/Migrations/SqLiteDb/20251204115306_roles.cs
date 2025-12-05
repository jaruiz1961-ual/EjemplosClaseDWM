using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataBase.Migrations.SqLiteDb
{
    /// <inheritdoc />
    public partial class roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Roles = table.Column<string>(type: "TEXT", nullable: false),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguridad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Contexto = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Seguridad",
                columns: new[] { "Id", "Email", "Password", "Roles", "TenantId", "UserName" },
                values: new object[,]
                {
                    { 1, null, "abc1", "Admin", 0, "admin1" },
                    { 2, null, "abc2", "Admin", 0, "admin2" },
                    { 3, null, "abc3", "Admin", 0, "admin3" },
                    { 4, null, "abc4", "Admin", 1, "admin4" },
                    { 5, null, "abc5", "Admin", 1, "admin5" },
                    { 6, null, "abc6", "Admin", 1, "admin6" },
                    { 7, null, "abc7", "Admin", 2, "admin7" },
                    { 8, null, "abc8", "Admin", 2, "admin8" },
                    { 9, null, "abc9", "Admin", 2, "admin9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seguridad");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
