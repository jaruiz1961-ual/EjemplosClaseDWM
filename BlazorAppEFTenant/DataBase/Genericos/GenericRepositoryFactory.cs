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

        public IGenericRepository<TEntity, TContext> Create<TEntity, TContext>(TContext context, IContextKeyProvider cp )
            where TEntity : class
            where TContext : DbContext
        {
            if (cp.ApiName == null )
            {
                var httpClient = _provider.GetRequiredService<HttpClient>();
                Console.WriteLine("BaseAddress: " + httpClient.BaseAddress);
                var resource =  typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryApi<TEntity, TContext>(httpClient, cp, resource);
            }
            else
            if (cp.ApiName != null && cp.ApiName.ToLower() != "ef")
            {
                var httpClient = _provider.GetRequiredService<IHttpClientFactory>().CreateClient(cp.ApiName);
                Console.WriteLine("BaseAddress: " + httpClient.BaseAddress);
                var resource =  typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryApi<TEntity, TContext>(httpClient, cp, resource);
            }
            else
            if (cp.ApiName.ToLower() == "ef")
                switch (cp.CurrentContextKey.ToLower())
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
                        throw new NotSupportedException($"Contexto '{cp.CurrentContextKey}' no soportado.");
                }
            throw new NotSupportedException($"Tipo de acceso '{cp.ApiName}' no soportado.");
        }

       
    }

}
