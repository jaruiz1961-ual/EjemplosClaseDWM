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

        public UnitOfWorkFactory(IServiceProvider provider, IGenericRepositoryFactory repoFactory, bool isApi = false)
        {
            _provider = provider;
            _repoFactory = repoFactory;
        }

        public IUnitOfWork Create(string contextoKey,bool isApi = false)
        {
            var tenant = _provider.GetRequiredService<ITenantProvider>();
            if (isApi)
            {
                var httpClient = _provider.GetRequiredService<HttpClient>();
                return new UnitOfWorkApi(httpClient, contextoKey); // Usa object si no hay contexto real
            }
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
                    default: throw new NotSupportedException($"Contexto '{contextoKey}' no soportado.");
            }
        }
    }


}
