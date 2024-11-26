﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlazorEbi7.Data.Migrations.MigrationsSqlServer
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsSqlServer())
            {
                migrationBuilder.CreateTable(
                    name: "UsuarioSet",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                        NivelAcceso = table.Column<int>(type: "int", nullable: false),
                        Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        email = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
