using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Shares.Genericos;
using System.Linq.Expressions;
  using Microsoft.Extensions.Configuration;
using Shares.Modelo;

namespace BlazorSeguridad2026.Data
{


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }

        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        //   : base(options)
        //{ }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            TenantSaveChangesInterceptor tenantInterceptor)
            : base(options)
        {
            _tenantInterceptor = tenantInterceptor;
            TenantId = _tenantInterceptor.ContextProvider._AppState.TenantId;
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
            ModelCreatingTenant(modelBuilder);

        }

        public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                TenantSaveChangesInterceptor tenantInterceptor = null;
                // ruta al appsettings del proyecto host (ajusta según tu solución)
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var connectionString = config.GetConnectionString("DefaultConnection");

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                return new ApplicationDbContext(optionsBuilder.Options, null);
            }
        }

    }
}
