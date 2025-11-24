using DataBase.Contextos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
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

        public IGenericRepository<TEntity, TContext> Create<TEntity, TContext>(string dbContextKey, TContext context, bool isApi,string apiResourceName = null)
            where TEntity : class
            where TContext : DbContext
        {
            if (isApi) 
            {
                var httpClient= _provider.GetRequiredService<IHttpClientFactory>().CreateClient("ApiRest");
                 Console.WriteLine("BaseAddress: " + httpClient.BaseAddress);
                var resource = apiResourceName ?? typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryApi<TEntity, TContext>(httpClient, dbContextKey, resource);
            }
            switch (dbContextKey.ToLower())
            {
                case "sqlserver":
                    var sqlDb = context as SqlServerDbContext;
                    return new GenericRepository<TEntity, SqlServerDbContext>(sqlDb) as IGenericRepository<TEntity, TContext>;
                case "sqlite":
                    var sqliteDb = context as SqLiteDbContext;
                    return new GenericRepository<TEntity, SqLiteDbContext>(sqliteDb) as IGenericRepository<TEntity, TContext>;
                case "inmemory":
                    var memDb = context as InMemoryDbContext;
                    return new GenericRepository<TEntity, InMemoryDbContext>(memDb) as IGenericRepository<TEntity, TContext>;
                    default:
                    throw new NotSupportedException($"Contexto '{dbContextKey}' no soportado.");
            }
        }

       
    }

}
