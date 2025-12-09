using Shares.Contextos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shares.Genericos
{

    public class GenericRepositoryFactory<TEntity> : IGenericRepositoryFactory<TEntity>
    where TEntity : class, IEntity
    {
        private readonly IServiceProvider _provider;

        public GenericRepositoryFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IGenericRepository<TEntity> Create(IContextProvider cp, DbContext? context = null)
        {
            var mode = cp._AppState.ConnectionMode?.ToLowerInvariant();



            if (mode == "api")
            {
                var httpClientFactory = _provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(cp._AppState.ApiName);

                var resource = typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryApi<TEntity>(httpClient, cp, resource);
            }

            if (mode == "ef")
            {
                if (context is null)
                    throw new ArgumentNullException(nameof(context), "Para EF se requiere un DbContext.");

                switch (cp._AppState.DbKey?.ToLowerInvariant())
                {
                    case "sqlserver":
                        {
                            var sqlServerDbContext = context as SqlServerDbContext;
                            return new GenericRepositoryEF<TEntity, SqlServerDbContext>(sqlServerDbContext);
                        }

                    case "sqlite":
                        {
                            var sqLiteDbContext = context as SqLiteDbContext;
                            return new GenericRepositoryEF<TEntity, SqLiteDbContext>(sqLiteDbContext);
                        }

                    case "inmemory":
                        {
                            var inMemoryDbContext = context as InMemoryDbContext;
                            return new GenericRepositoryEF<TEntity, InMemoryDbContext> (inMemoryDbContext);
                        }

                    default:
                        throw new NotSupportedException($"Contexto '{cp._AppState.DbKey}' no soportado.");
                }
            }

            throw new NotSupportedException($"Tipo de acceso '{cp._AppState.ConnectionMode}' no soportado.");
        }
    }


}
