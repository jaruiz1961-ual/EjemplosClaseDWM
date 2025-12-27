//#define UPDATE_DATABASE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Data.Modelo;



namespace BlazorSeguridad2026.Base.Contextos
{
    public class BaseDbContext : DbContext
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }



        public BaseDbContext(DbContextOptions<SqlServerDbContext> options, TenantSaveChangesInterceptor tenantInterceptor) 
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
            ModelCreating(modelBuilder);
            ModelCreatingTenant(modelBuilder);

        }

        protected virtual void ModelCreating(ModelBuilder modelBuilder)
        {
        }




    }

}
