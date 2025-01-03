﻿// <auto-generated />
using BlazorAppScaff.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlazorAppScaff.Migrations
{
    [DbContext(typeof(TestContext))]
    [Migration("20241119114219_Inicial")]
    partial class Inicial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BlazorAppScaff.Models.Alumno", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellidos")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dni")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("EmailUal")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Email-ual");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsernameUal")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Username-ual");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Dni" }, "IX_DniAlumno")
                        .IsUnique()
                        .HasFilter("[Dni] IS NOT NULL");

                    b.ToTable("Alumnos");
                });

            modelBuilder.Entity("BlazorAppScaff.Models.AlumnosAsignatura", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AlumnosId")
                        .HasColumnType("int");

                    b.Property<int>("AsignaturaId")
                        .HasColumnType("int");

                    b.Property<string>("CursoAcademico")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("AlumnosId");

                    b.HasIndex("AsignaturaId");

                    b.ToTable("AlumnosAsignaturas");
                });

            modelBuilder.Entity("BlazorAppScaff.Models.Asignatura", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Clave")
                        .HasMaxLength(20)
                        .HasColumnType("nchar(20)")
                        .IsFixedLength();

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Clave" }, "IX_ClaveCursos")
                        .IsUnique()
                        .HasFilter("[Clave] IS NOT NULL");

                    b.ToTable("Asignaturas");
                });

            modelBuilder.Entity("BlazorAppScaff.Models.AlumnosAsignatura", b =>
                {
                    b.HasOne("BlazorAppScaff.Models.Alumno", "Alumnos")
                        .WithMany("AlumnosAsignaturas")
                        .HasForeignKey("AlumnosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_AlumnosAsignaturas_Alumnos");

                    b.HasOne("BlazorAppScaff.Models.Asignatura", "Asignatura")
                        .WithMany("AlumnosAsignaturas")
                        .HasForeignKey("AsignaturaId")
                        .IsRequired()
                        .HasConstraintName("FK_AlumnosAsignaturas_Asignaturas");

                    b.Navigation("Alumnos");

                    b.Navigation("Asignatura");
                });

            modelBuilder.Entity("BlazorAppScaff.Models.Alumno", b =>
                {
                    b.Navigation("AlumnosAsignaturas");
                });

            modelBuilder.Entity("BlazorAppScaff.Models.Asignatura", b =>
                {
                    b.Navigation("AlumnosAsignaturas");
                });
#pragma warning restore 612, 618
        }
    }
}
