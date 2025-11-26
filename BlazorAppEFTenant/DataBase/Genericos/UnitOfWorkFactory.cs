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

        public IUnitOfWork Create(IContextProvider cp)
        {
      
     
            if (cp.ConnectionMode.ToLower() == "apiclient")
            {
                return new UnitOfWorkApi<DbContext>(cp, _repoFactory);
            }
            else
            if (cp.ConnectionMode.ToLower() == "apiserver")
            {
                return new UnitOfWorkApi<DbContext>(cp, _repoFactory);
            }
            else if (cp.ConnectionMode.ToLower() == "ef")
                switch (cp.DbKey.ToLower())
                {
                    case "sqlserver":
                        var sqlDb = _provider.GetRequiredService<SqlServerDbContext>();

                        return new UnitOfWork<SqlServerDbContext>(sqlDb, cp, _repoFactory);
                    case "sqlite":
                        var sqLite = _provider.GetRequiredService<SqLiteDbContext>();
                        return new UnitOfWork<SqLiteDbContext>(sqLite, cp, _repoFactory);
                    case "inmemory":
                        var inMemory = _provider.GetRequiredService<InMemoryDbContext>();
                        return new UnitOfWork<InMemoryDbContext>(inMemory, cp, _repoFactory);
                    default: throw new NotSupportedException($"Contexto '{cp.DbKey}' no soportado.");
                }
            throw new NotSupportedException($"Tipo de acceso '{cp.ConnectionMode}' no soportado.");
        }
    }


}
