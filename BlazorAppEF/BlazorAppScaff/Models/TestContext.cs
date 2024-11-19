using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppScaff.Models;

public partial class TestContext : DbContext
{
    public TestContext()
    {
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumnos { get; set; }

    public virtual DbSet<AlumnosAsignatura> AlumnosAsignaturas { get; set; }

    public virtual DbSet<Asignatura> Asignaturas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MssqlLocaldb;Initial Catalog=Test;AttachDbFileName=C:\\Temp\\TestDb.mdf ;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasIndex(e => e.Dni, "IX_DniAlumno").IsUnique();

            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.Dni)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.EmailUal).HasColumnName("Email-ual");
            entity.Property(e => e.UsernameUal).HasColumnName("Username-ual");
        });

        modelBuilder.Entity<AlumnosAsignatura>(entity =>
        {
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.CursoAcademico)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Alumnos).WithMany(p => p.AlumnosAsignaturas)
                .HasForeignKey(d => d.AlumnosId)
                .HasConstraintName("FK_AlumnosAsignaturas_Alumnos");

            entity.HasOne(d => d.Asignatura).WithMany(p => p.AlumnosAsignaturas)
                .HasForeignKey(d => d.AsignaturaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlumnosAsignaturas_Asignaturas");
        });

        modelBuilder.Entity<Asignatura>(entity =>
        {
            entity.HasIndex(e => e.Clave, "IX_ClaveCursos").IsUnique();

            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.Clave)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

     void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alumno>().HasData(
       new Alumno { Id = 1, Nombre = "jose", Dni = "1234564", EmailUal = "jose@ual.es",
           UsernameUal = "jam3434", Apellidos = "moreno", Telefono = "67646376" },
        new Alumno
        {
            Id = 2,
            Nombre = "juan",
            Dni = "9876564",
            EmailUal = "juan@ual.es",
            UsernameUal = "amt677",
            Apellidos = "lopez",
            Telefono = "67123408"
        });

    }
}
