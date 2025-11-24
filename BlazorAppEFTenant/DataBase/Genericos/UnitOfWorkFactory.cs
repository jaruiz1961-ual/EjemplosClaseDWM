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

        public UnitOfWorkFactory(IServiceProvider provider, IGenericRepositoryFactory repoFactory)
        {
            _provider = provider;
            _repoFactory = repoFactory;
        }

        public IUnitOfWork Create(string contextoKey)
        {
            var tenant = _provider.GetRequiredService<ITenantProvider>();
            switch (contextoKey)
            {
                case "SqlServer":
                    var sqlDb = _provider.GetRequiredService<SqlServerDbContext>();

                    return new UnitOfWork<SqlServerDbContext>(sqlDb, tenant, _repoFactory, contextoKey);
                case "SqLite":
                    var sqLite = _provider.GetRequiredService<SqLiteDbContext>();
                    return new UnitOfWork<SqLiteDbContext>(sqLite, tenant, _repoFactory, contextoKey);
                case "InMemory":
                    var inMemory = _provider.GetRequiredService<InMemoryDbContext>();
                    return new UnitOfWork<InMemoryDbContext>(inMemory, tenant, _repoFactory, contextoKey);
                case "Api":
                    var httpClient = _provider.GetRequiredService<HttpClient>();
                    return new UnitOfWorkApi(httpClient,contextoKey); // Usa object si no hay contexto real
                                                                                               // Otras variantes...
                default:
                    var inM = _provider.GetRequiredService<InMemoryDbContext>();
                    return new UnitOfWork<InMemoryDbContext>(inM, tenant, _repoFactory, contextoKey);
            }
        }
    }


}
