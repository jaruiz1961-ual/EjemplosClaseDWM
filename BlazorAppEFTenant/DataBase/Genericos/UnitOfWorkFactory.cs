using DataBase.Contextos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider _provider;
        private readonly IGenericRepositoryFactory _repoFactory;
        private readonly bool _isApi;

        public UnitOfWorkFactory(IServiceProvider provider, IGenericRepositoryFactory repoFactory)
        {
            _provider = provider;
            _repoFactory = repoFactory;
        }

        public IUnitOfWork Create(IContextKeyProvider cp)
        {
            var tenantProvider = _provider.GetRequiredService<ITenantProvider>();
            string apiResource = null;
            if (cp.ApiName == null)
            {
                return new UnitOfWorkApi<DbContext>(null, tenantProvider, _repoFactory, cp);
            }
            else
            if (cp.ApiName != null && cp.ApiName.ToLower() != "ef")
            {
                return new UnitOfWorkApi<DbContext>(null, tenantProvider, _repoFactory, cp);
            }
            else if (cp.ApiName.ToLower() == "ef")
                switch (cp.CurrentContextKey.ToLower())
                {
                    case "sqlserver":
                        var sqlDb = _provider.GetRequiredService<SqlServerDbContext>();

                        return new UnitOfWork<SqlServerDbContext>(sqlDb, tenantProvider, _repoFactory, cp);
                    case "sqlite":
                        var sqLite = _provider.GetRequiredService<SqLiteDbContext>();
                        return new UnitOfWork<SqLiteDbContext>(sqLite, tenantProvider, _repoFactory, cp);
                    case "inmemory":
                        var inMemory = _provider.GetRequiredService<InMemoryDbContext>();
                        return new UnitOfWork<InMemoryDbContext>(inMemory, tenantProvider, _repoFactory, cp);
                    default: throw new NotSupportedException($"Contexto '{cp.CurrentContextKey}' no soportado.");
                }
            throw new NotSupportedException($"Tipo de acceso '{cp.ApiName}' no soportado.");
        }
    }


}
