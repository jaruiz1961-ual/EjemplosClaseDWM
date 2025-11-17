using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class SqlServerContextFactory : IDesignTimeDbContextFactory<SqlServerContext>
    {
        public SqlServerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Nueva2; AttachDbFilename=c:\temp\Nueva2.db ;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new SqlServerContext(optionsBuilder.Options);
        }
    }

    public class SqlServerContext : DbContext
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int CurrentTenantId;

        public SqlServerContext(DbContextOptions<SqlServerContext> options)
            : base(options)
        {

        }

        public SqlServerContext(DbContextOptions<SqlServerContext> options, TenantSaveChangesInterceptor tenantInterceptor)
            : base(options)
        {
            _tenantInterceptor = tenantInterceptor;
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
                    // Construir expresión para e => ((ITenantEntity)e).TenantId == CurrentTenantId
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
                    var currentTenantId = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));
                    var equals = Expression.Equal(property, currentTenantId);
                    var lambda = Expression.Lambda(equals, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
          
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
            ModelCreatingTenant(modelBuilder);
            modelBuilder.Entity<Usuario>()
               .HasQueryFilter(u => u.TenantId == 0 || ( u.TenantId == CurrentTenantId));

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.Codigo).IsRequired();
                entity.Property(e => e.UserName).IsRequired();
            });

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Id = 1, UserName = "Usuario1", NivelAcceso = 1, Codigo = "0001", Password = "abc 11", TenantId = 1 },
                new Usuario { Id = 2, UserName = "Usuario2", NivelAcceso = 1, Codigo = "0002", Password = "abc 22", TenantId = 2 },
                new Usuario { Id = 3, UserName = "Usuario3", NivelAcceso = 1, Codigo = "0003", Password = "abc 33", TenantId = 0 }
            );


        }
    }

}
