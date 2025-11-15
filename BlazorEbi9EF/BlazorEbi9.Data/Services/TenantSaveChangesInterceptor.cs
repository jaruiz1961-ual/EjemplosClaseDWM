
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorEbi9.Data.Services
{
    public class TenantSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceProvider _rootProvider;

        public TenantSaveChangesInterceptor(IServiceProvider rootProvider)
        {
            _rootProvider = rootProvider ?? throw new ArgumentNullException(nameof(rootProvider));
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyTenant(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyTenant(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyTenant(DbContext? context)
        {
            if (context == null) return;

            using var scope = _rootProvider.CreateScope();
            var tenantProvider = scope.ServiceProvider.GetService<BlazorEbi9.Model.TenantService.ITenantProvider>();
            if (tenantProvider == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is BlazorEbi9.Model.Entidades.ITenantEntity && e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                var tenantEntity = (BlazorEbi9.Model.Entidades.ITenantEntity)entry.Entity;
                tenantEntity.TenantId = tenantProvider.TenantId;
            }
        }
    }
}