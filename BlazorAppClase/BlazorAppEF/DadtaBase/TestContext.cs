using BlazorAppEF.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppEF.DadtaBase
{
    public partial class TestContext : DbContext
    {
        public TestContext() { }
        public TestContext(DbContextOptions<TestContext> options) : base(options) { }
        public virtual DbSet<Alumno> Alumnos { get; set; }
        public virtual DbSet<AlumnoAsignatura> AlumnosAsignaturas { get; set; }
        public virtual DbSet<Asignatura> Asignaturas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MssqlLocaldb;Initial Catalog=Test2;AttachDbFileName=C:\\Temp\\Test2Db.mdf ;Integrated Security=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alumno>(entity => { entity.Property(e => e.Id).UseIdentityColumn(); });
            modelBuilder.Entity<Asignatura>(entity => { entity.Property(e => e.Id).UseIdentityColumn(); });
            modelBuilder.Entity<AlumnoAsignatura>(entity => {
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.HasOne(d => d.Alumno)
                .WithMany(p => p.AlumnosAsignaturas)
                .HasForeignKey(d => d.AlumnoId);
                entity.HasOne(d => d.Asignatura)
                .WithMany(p => p.AlumnosAsignaturas)
                .HasForeignKey(d => d.AsignaturaId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }

}
