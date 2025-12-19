using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using BlazorSeguridad2026.Data;
using Shares.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Genericos
{
    public class TenantSaveChangesInterceptor : SaveChangesInterceptor
    {
        public IContextProvider ContextProvider { get; set; }

        public TenantSaveChangesInterceptor(IContextProvider tenantProvider)
        {
            ContextProvider = tenantProvider;
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
            var tenantId = ContextProvider._AppState.TenantId;

            foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added && tenantId != null)
                {
                    entry.Entity.TenantId = tenantId;
                }
            }
        }
    }

}
