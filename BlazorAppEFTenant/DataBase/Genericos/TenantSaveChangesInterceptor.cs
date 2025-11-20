using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class TenantSaveChangesInterceptor : SaveChangesInterceptor
    {
        public ITenantServices TenantService { get; set; }

        public TenantSaveChangesInterceptor(ITenantServices tenantService)
        {
            TenantService = tenantService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            AssignTenantId(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AssignTenantId(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AssignTenantId(DbContext? context)
        {
            if (context == null) return;
            var tenantId = TenantService.CurrentTenantId;

            foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added )
                {
                    entry.Entity.TenantId = tenantId;
                }
            }
        }
    }

}
