﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlazorEbi7.Data.Migrations.MigrationsSqlServer
{
    [DbContext(typeof(SqlServerDbContext))]
    partial class SqlServerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BlazorEbi7.Model.Entidades.UsuarioSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NivelAcceso")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UsuarioSet");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Codigo = "0001",
                            NivelAcceso = 1,
                            Password = "abc 11",
                            UserName = "Usuario1"
                        },
                        new
                        {
                            Id = 2,
                            Codigo = "0002",
                            NivelAcceso = 1,
                            Password = "abc 22",
                            UserName = "Usuario2"
                        },
                        new
                        {
                            Id = 3,
                            Codigo = "0003",
                            NivelAcceso = 1,
                            Password = "abc 33",
                            UserName = "Usuario3"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
