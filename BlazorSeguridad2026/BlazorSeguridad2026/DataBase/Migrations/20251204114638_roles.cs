using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataBase.Migrations
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguridad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
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
                    { 1, null, "abc1", "User", 0, "admin1" },
                    { 2, null, "abc2", "User", 0, "admin2" },
                    { 3, null, "abc3", "User", 0, "admin3" },
                    { 4, null, "abc4", "User", 1, "admin4" },
                    { 5, null, "abc5", "User", 1, "admin5" },
                    { 6, null, "abc6", "User", 1, "admin6" },
                    { 7, null, "abc7", "User", 2, "admin7" },
                    { 8, null, "abc8", "User", 2, "admin8" },
                    { 9, null, "abc9", "User", 2, "admin9" }
                });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "Id", "Codigo", "Contexto", "Password", "TenantId", "UserName", "email" },
                values: new object[,]
                {
                    { 1, "0001", "SqlServer", "abc 11", 0, "Usuario1", null },
                    { 2, "0002", "SqlServer", "abc 22", 1, "Usuario2", null },
                    { 3, "0003", "SqlServer", "abc 33", 2, "Usuario3", null }
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
