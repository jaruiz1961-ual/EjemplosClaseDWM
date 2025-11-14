using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorEbi9.Data.Migrations.MigrationsSqLite
{
    /// <inheritdoc />
    public partial class tenentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "UsuarioSet",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "UsuarioSet",
                keyColumn: "Id",
                keyValue: 1,
                column: "TenantId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "UsuarioSet",
                keyColumn: "Id",
                keyValue: 2,
                column: "TenantId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "UsuarioSet",
                keyColumn: "Id",
                keyValue: 3,
                column: "TenantId",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UsuarioSet");
        }
    }
}
