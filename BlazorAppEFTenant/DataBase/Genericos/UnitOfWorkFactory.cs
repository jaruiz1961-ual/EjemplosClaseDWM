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

        public IUnitOfWork Create(string contextoKey,bool isApi )
        {
            var tenant = _provider.GetRequiredService<ITenantProvider>();
            if (isApi)
            {
                return new UnitOfWork<DbContext>(null, tenant, _repoFactory, contextoKey, true);
            }
            switch (contextoKey.ToLower())
            {
                case "sqlserver":
                    var sqlDb = _provider.GetRequiredService<SqlServerDbContext>();

                    return new UnitOfWork<SqlServerDbContext>(sqlDb, tenant, _repoFactory, contextoKey,false);
                case "sqlite":
                    var sqLite = _provider.GetRequiredService<SqLiteDbContext>();
                    return new UnitOfWork<SqLiteDbContext>(sqLite, tenant, _repoFactory, contextoKey,false);
                case "inmemory":
                    var inMemory = _provider.GetRequiredService<InMemoryDbContext>();
                    return new UnitOfWork<InMemoryDbContext>(inMemory, tenant, _repoFactory, contextoKey,false);
                    default: throw new NotSupportedException($"Contexto '{contextoKey}' no soportado.");
            }
        }
    }


}
