using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Seguridad : Migration
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
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
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
        }
    }
}
