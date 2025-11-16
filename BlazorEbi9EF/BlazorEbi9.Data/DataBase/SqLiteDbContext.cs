
using BlazorEbi9.Model.Entidades;
using Microsoft.EntityFrameworkCore;
using System;

namespace BlazorEbi9.Data.DataBase
{
    public partial class SqLiteDbContext : DbContext
    {
 
        // Método para establecer el tenant en tiempo de ejecución
        public int? CurrentTenant { get; set; }

        public SqLiteDbContext(DbContextOptions<SqLiteDbContext> opts) : base(opts)
        {
        }

        public virtual DbSet<UsuarioSet>? UsuarioSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Expresión simple y traducible:
            // permite filas públicas (TenantId == 0) o filas del tenant actual cuando _currentTenant tiene valor
            modelBuilder.Entity<UsuarioSet>()
                .HasQueryFilter(u => u.TenantId == 0 || (CurrentTenant != null && u.TenantId == CurrentTenant.Value));

            modelBuilder.Entity<UsuarioSet>(entity =>
            {
                entity.Property(e => e.Codigo).IsRequired();
                entity.Property(e => e.UserName).IsRequired();
            });

            modelBuilder.Entity<UsuarioSet>().HasData(
                new UsuarioSet { Id = 1, UserName = "Usuario1", NivelAcceso = 1, Codigo = "0001", Password = "abc 11", TenantId = 1 },
                new UsuarioSet { Id = 2, UserName = "Usuario2", NivelAcceso = 1, Codigo = "0002", Password = "abc 22", TenantId = 2 },
                new UsuarioSet { Id = 3, UserName = "Usuario3", NivelAcceso = 1, Codigo = "0003", Password = "abc 33", TenantId = 0 }
            );
        }
    }
}