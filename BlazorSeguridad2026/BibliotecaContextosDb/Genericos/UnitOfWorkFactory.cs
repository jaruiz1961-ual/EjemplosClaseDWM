
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Base.Contextos;


namespace BlazorSeguridad2026.Base.Genericos
{

    public interface IUnitOfWorkFactory
    {
        IUnitOfWorkAsync Create(IContextProvider cp);
    }
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider _provider;
      

        public UnitOfWorkFactory(IServiceProvider provider, IContextProvider cp)
        {
            _provider = provider;
      
        }

        public IUnitOfWorkAsync Create(IContextProvider cp)
        {
            
            if (cp._AppState.ConnectionMode.ToLower() == "api")
            {
                return new UnitOfWorkAsync(cp, _provider);
            }
            else if (cp._AppState.ConnectionMode.ToLower() == "ef")
                switch (cp._AppState.DbKey.ToLower())
                {
                    case "application":
                        var AppFactorySqlServer = _provider.GetRequiredService<IDbContextFactory<ApplicationBaseDbContext>>();
                        ApplicationBaseDbContext AppSqlServer = AppFactorySqlServer.CreateDbContext(); // nuevo contexto por llamada
                        AppSqlServer.TenantId = cp._AppState.TenantId;
                        return new UnitOfWorkEfAsync<ApplicationBaseDbContext>(AppSqlServer, _provider, cp);
                    case "sqlserver":
                        var dbFactorySqlServer = _provider.GetRequiredService<IDbContextFactory<SqlServerDbContext>>();
                        SqlServerDbContext dbSqlServer = dbFactorySqlServer.CreateDbContext(); // nuevo contexto por llamada
                        dbSqlServer.TenantId = cp._AppState.TenantId;
                        return new UnitOfWorkEfAsync<SqlServerDbContext>(dbSqlServer,_provider, cp );
                  
                    case "sqlite":
                        var dbFactorySqLite = _provider.GetRequiredService<IDbContextFactory<SqLiteDbContext>>();
                        SqLiteDbContext dbSqLite = dbFactorySqLite.CreateDbContext(); // nuevo contexto por llamada
                        dbSqLite.TenantId = cp._AppState.TenantId;
                        return new UnitOfWorkEfAsync<SqLiteDbContext>(dbSqLite, _provider, cp);
                    case "inmemory":
                        var dbFactoryInMemory = _provider.GetRequiredService<IDbContextFactory<InMemoryBaseDbContext>>();
                        InMemoryBaseDbContext dbInMemory = dbFactoryInMemory.CreateDbContext(); // nuevo contexto por llamada
                        dbInMemory.TenantId = cp._AppState.TenantId;
                        return new UnitOfWorkEfAsync<InMemoryBaseDbContext>(dbInMemory, _provider, cp);
                    default: throw new NotSupportedException($"Contexto '{cp._AppState.DbKey}' no soportado.");
                }
            throw new NotSupportedException($"Tipo de acceso '{cp._AppState.ConnectionMode}' no soportado.");
        }
    }


}
