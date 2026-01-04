
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Data.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSeguridad2026.Base.Contextos
{



    public class InMemoryBaseDbContext : DbContext
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }


        public InMemoryBaseDbContext(DbContextOptions<InMemoryBaseDbContext> options, TenantSaveChangesInterceptor tenantInterceptor) 
            : base(options) 
        {
            _tenantInterceptor = tenantInterceptor;
            State? estado = _tenantInterceptor.ContextProvider.GetState();
            TenantId = estado.TenantId;

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_tenantInterceptor);
            base.OnConfiguring(optionsBuilder);
        }

        protected void ModelCreatingTenant(ModelBuilder modelBuilder)
        {
            var tenantEntityType = typeof(ITenantEntity);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (tenantEntityType.IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(ITenantEntity.TenantId));

                    // this.TenantId
                    var currentTenantId = Expression.Property(
                        Expression.Constant(this),
                        nameof(TenantId));

                    var equals = Expression.Equal(property, currentTenantId);
                    var lambda = Expression.Lambda(equals, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.Codigo).IsRequired();
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Usuario>().HasData
       (new Usuario { Id = 1, UserName = "Usuario1", Contexto = "InMemory", Codigo = "0001", Password = "abc 11", TenantId = 0 },
       new Usuario { Id = 2, UserName = "Usuario2", Contexto = "InMemory", Codigo = "0002", Password = "abc 22", TenantId = 1 },
       new Usuario { Id = 3, UserName = "Usuario3", Contexto = "InMemory", Codigo = "0003", Password = "abc 33", TenantId = 2 });




            ModelCreatingTenant(modelBuilder);

        }

        public virtual DbSet<Usuario>? Usuario { get; set; }


    }

}
