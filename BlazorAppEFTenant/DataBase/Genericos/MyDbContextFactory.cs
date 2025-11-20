using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public interface IMyDbContextFactory
    {
        DbContext CreateDbContext(string contextoKey, ITenantServices Tenant);
    }
    public class MyDbContextFactory : IMyDbContextFactory
    {
        private readonly IServiceProvider _provider;
        public MyDbContextFactory(IServiceProvider provider, ITenantServices tenant) => _provider = provider;

        public DbContext CreateDbContext(string key, ITenantServices Tenant)
        {
            switch (key)
            {
                case "SqlServerContext":
                    var ctx = _provider.GetRequiredService<IDbContextFactory<SqlServerContext>>().CreateDbContext();
                    ctx.CurrentTenantId = Tenant.CurrentTenantId;
                    return ctx;
                // otros contextos...
                default: throw new NotSupportedException();
            }
        }

    }

}
