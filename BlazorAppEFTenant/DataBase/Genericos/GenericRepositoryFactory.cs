using DataBase.Contextos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class GenericRepositoryFactory : IGenericRepositoryFactory
    {
        private readonly IServiceProvider _provider;

        public GenericRepositoryFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IGenericRepository<TEntity, TContext> Create<TEntity, TContext>(string dbContextKey, bool isApi,string apiResourceName = null)
            where TEntity : class
            where TContext : DbContext
        {
            if (isApi) 
            {
                var httpClient = _provider.GetRequiredService<IHttpClientFactory>().CreateClient("ApiRest");
                var resource = apiResourceName ?? typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryApi<TEntity, TContext>(httpClient, dbContextKey, resource);
            }
            switch (dbContextKey)
            {
                case "SqlServer":
                    var sqlDb = _provider.GetRequiredService<SqlServerDbContext>();
                    return new GenericRepository<TEntity, SqlServerDbContext>(sqlDb) as IGenericRepository<TEntity, TContext>;
                case "SqLite":
                    var sqliteDb = _provider.GetRequiredService<SqLiteDbContext>();
                    return new GenericRepository<TEntity, SqLiteDbContext>(sqliteDb) as IGenericRepository<TEntity, TContext>;
                case "InMemory":
                    var memDb = _provider.GetRequiredService<InMemoryDbContext>();
                    return new GenericRepository<TEntity, InMemoryDbContext>(memDb) as IGenericRepository<TEntity, TContext>;
                    default:
                    throw new NotSupportedException($"Contexto '{dbContextKey}' no soportado.");
            }
        }

       
    }

}
