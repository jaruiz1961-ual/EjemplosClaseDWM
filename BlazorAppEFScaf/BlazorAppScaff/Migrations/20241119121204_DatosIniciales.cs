using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlazorAppScaff.Migrations
{
    /// <inheritdoc />
    public partial class DatosIniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Alumnos",
                columns: new[] { "Id", "Apellidos", "Dni", "Email-ual", "Nombre", "Telefono", "Username-ual" },
                values: new object[,]
                {
                    { 1, "moreno", "1234564", "jose@ual.es", "jose", "67646376", "jam3434" },
                    { 2, "lopez", "9876564", "juan@ual.es", "juan", "67123408", "amt677" }
                });

            migrationBuilder.InsertData(
                table: "Asignaturas",
                columns: new[] { "Id", "Clave", "Nombre" },
                values: new object[,]
                {
                    { 1, "DWM", "desasrrollo web movil" },
                    { 2, "PSS", "Programacion Servicios" }
                });

            migrationBuilder.InsertData(
                table: "AlumnosAsignaturas",
                columns: new[] { "Id", "AlumnosId", "AsignaturaId", "CursoAcademico" },
                values: new object[,]
                {
                    { 1, 1, 1, null },
                    { 2, 1, 2, null },
                    { 3, 2, 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlumnosAsignaturas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AlumnosAsignaturas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AlumnosAsignaturas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Alumnos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Alumnos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Asignaturas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Asignaturas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
