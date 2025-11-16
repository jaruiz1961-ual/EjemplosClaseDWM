using BlazorEbi9.Model.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorEbi9.Data.Services
{
    public class TenantSaveChangesInterceptor : SaveChangesInterceptor
    {
        public TenantSaveChangesInterceptor()
        {
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

            // 1) Intentar obtener el tenant directamente desde el DbContext si la instancia lo expone
            int? tenant = null;
            if (context is BlazorEbi9.Data.DataBase.SqLiteDbContext sqCtx)
            {
                // si usas la propiedad pública CurrentTenant:
                tenant = sqCtx.CurrentTenant;
                // o si usas SetCurrentTenant/_currentTenant, añade aquí un getter público y úsalo
            }

            // 2) Si no hay tenant en la instancia, resolver ITenantProvider desde el IServiceProvider asociado al DbContext
            //    (esto obtiene servicios del mismo scope que creó el DbContext)
            if (tenant == null)
            {
                try
                {
                    var scopedProvider = ((IInfrastructure<IServiceProvider>)context).Instance;
                    var tenantProvider = scopedProvider.GetService<BlazorEbi9.Model.TenantService.ITenantProvider>();
                    if (tenantProvider != null)
                    {
                        tenant = tenantProvider.TenantId;
                    }
                }
                catch
                {
                    // no forzar excepción aquí; simplemente no aplicamos tenant si no se puede resolver
                    tenant = null;
                }
            }

            if (tenant == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is ITenantEntity /* marca tu interfaz */);

            foreach (var entry in entries)
            {
                var tenantEntity = (ITenantEntity)entry.Entity;
                // Sólo asignar si es 0 o si quieres forzar siempre:
                tenantEntity.TenantId = tenant.Value;
            }
        }
    }
}