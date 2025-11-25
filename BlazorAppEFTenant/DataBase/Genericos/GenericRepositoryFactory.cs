using DataBase.Contextos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataBase.Genericos
{

    public class GenericRepositoryFactory : IGenericRepositoryFactory
    {
        private readonly IServiceProvider _provider;

        public GenericRepositoryFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IGenericRepository<TEntity, TContext> Create<TEntity, TContext>(string dbContextKey, TContext context, string apiName,string apiResourceName )
            where TEntity : class
            where TContext : DbContext
        {
            if (apiName == null )
            {
                var httpClient = _provider.GetRequiredService<IHttpClientFactory>().CreateClient(apiName);
                Console.WriteLine("BaseAddress: " + httpClient.BaseAddress);
                var resource = apiResourceName ?? typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryApi<TEntity, TContext>(httpClient, dbContextKey, resource);
            }
            else
            if (apiName != null && apiName.ToLower() != "ef")
            {
                var httpClient = _provider.GetRequiredService<IHttpClientFactory>().CreateClient(apiName);
                Console.WriteLine("BaseAddress: " + httpClient.BaseAddress);
                var resource = apiResourceName ?? typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryApi<TEntity, TContext>(httpClient, dbContextKey, resource);
            }
            else
            if (apiName.ToLower() == "ef")
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
            throw new NotSupportedException($"Tipo de acceso '{apiName}' no soportado.");
        }

       
    }

}
