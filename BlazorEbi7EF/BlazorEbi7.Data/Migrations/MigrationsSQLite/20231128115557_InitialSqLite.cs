using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlazorEbi7.Data.Migrations.MigrationsSqLite
{
    /// <inheritdoc />
    public partial class InitialSqLite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsSqlite())
            {
                migrationBuilder.CreateTable(
                    name: "UsuarioSet",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "INTEGER", nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        Codigo = table.Column<string>(type: "TEXT", nullable: false),
                        UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                        NivelAcceso = table.Column<int>(type: "INTEGER", nullable: false),
                        Password = table.Column<string>(type: "TEXT", nullable: false),
                        email = table.Column<string>(type: "TEXT", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_UsuarioSet", x => x.Id);
                    });

                migrationBuilder.InsertData(
                    table: "UsuarioSet",
                    columns: new[] { "Id", "Codigo", "NivelAcceso", "Password", "UserName", "email" },
                    values: new object[,]
                    {
                    { 1, "0001", 1, "abc 11", "Usuario1", null },
                    { 2, "0002", 1, "abc 22", "Usuario2", null },
                    { 3, "0003", 1, "abc 33", "Usuario3", null }
                    });
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioSet");
        }
    }
}
