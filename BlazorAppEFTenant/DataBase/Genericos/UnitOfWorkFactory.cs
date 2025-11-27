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
      

        public UnitOfWorkFactory(IServiceProvider provider, IContextProvider cp)
        {
            _provider = provider;
      
        }

        public IUnitOfWork Create(IContextProvider cp)
        {
            if (cp.ConnectionMode.ToLower() == "apiclient")
            {
                return new UnitOfWorkApi(cp, _provider);
            }
            else
            if (cp.ConnectionMode.ToLower() == "apiserver")
            {
                return new UnitOfWorkApi(cp, _provider);
            }
            else if (cp.ConnectionMode.ToLower() == "ef")
                switch (cp.DbKey.ToLower())
                {
                    case "sqlserver":
                        var dbFactorySqlServer = _provider.GetRequiredService<IDbContextFactory<SqlServerDbContext>>();
                        SqlServerDbContext dbSqlServer = dbFactorySqlServer.CreateDbContext(); // nuevo contexto por llamada
                        dbSqlServer.TenantId = cp.TenantId;
                        return new UnitOfWorkEf<SqlServerDbContext>(dbSqlServer,_provider, cp );
                  
                    case "sqlite":
                        var dbFactorySqLite = _provider.GetRequiredService<IDbContextFactory<SqLiteDbContext>>();
                        SqLiteDbContext dbSqLite = dbFactorySqLite.CreateDbContext(); // nuevo contexto por llamada
                        dbSqLite.TenantId = cp.TenantId;
                        return new UnitOfWorkEf<SqLiteDbContext>(dbSqLite, _provider, cp);
                    case "inmemory":
                        var dbFactoryInMemory = _provider.GetRequiredService<IDbContextFactory<InMemoryDbContext>>();
                        InMemoryDbContext dbInMemory = dbFactoryInMemory.CreateDbContext(); // nuevo contexto por llamada
                        dbInMemory.TenantId = cp.TenantId;
                        return new UnitOfWorkEf<InMemoryDbContext>(dbInMemory, _provider, cp);
                    default: throw new NotSupportedException($"Contexto '{cp.DbKey}' no soportado.");
                }
            throw new NotSupportedException($"Tipo de acceso '{cp.ConnectionMode}' no soportado.");
        }
    }


}
