
using System;
using System.Threading.Tasks;
using BlazorEbi9.Model.TenantService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorEbi9.Data.DataBase
{
    public interface ITenantDbContextFactory
    {
        Task<SqLiteDbContext> CreateDbContextAsync();
    }

    public class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly IDbContextFactory<SqLiteDbContext> _innerFactory;
        private readonly ITenantService _tenantService;

        public TenantDbContextFactory(IDbContextFactory<SqLiteDbContext> innerFactory, ITenantService tenantService)
        {
            _innerFactory = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
            _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        }

        public async Task<SqLiteDbContext> CreateDbContextAsync()
        {
            var ctx = await _innerFactory.CreateDbContextAsync();
            // Establecer el tenant actual para que el filtro lo use como parámetro
            ctx.SetCurrentTenant(_tenantService.Tenant);
            return ctx;
        }
    }
}