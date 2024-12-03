using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAppEF.Migrations
{
    /// <inheritdoc />
    public partial class movil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Alumnos",
                newName: "Movil");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Movil",
                table: "Alumnos",
                newName: "Telefono");
        }
    }
}
