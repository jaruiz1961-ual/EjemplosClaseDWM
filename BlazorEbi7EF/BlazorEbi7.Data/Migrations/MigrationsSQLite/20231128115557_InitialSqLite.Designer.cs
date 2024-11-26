﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlazorEbi7.Data.Migrations.MigrationsSqLite
{
    [DbContext(typeof(SqLiteDbContext))]
    [Migration("20231128115557_InitialSqLite")]
    partial class InitialSqLite
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("BlazorEbi7.Model.Entidades.UsuarioSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("NivelAcceso")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("email")
                        .HasColumnType("TEXT");

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
