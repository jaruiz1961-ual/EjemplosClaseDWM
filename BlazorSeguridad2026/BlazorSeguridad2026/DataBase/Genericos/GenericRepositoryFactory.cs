using Shares.Contextos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shares.Modelo;
using Shares.Seguridad;

namespace Shares.Genericos
{
    public interface IGenericRepositoryFactoryAsync<TEntity> where TEntity : class, IEntity
    {
        public IGenericRepositoryAsync<TEntity> Create(IContextProvider ic, DbContext context = null);

    }

    public class GenericRepositoryFactory<TEntity> : IGenericRepositoryFactoryAsync<TEntity>
    where TEntity : class, IEntity
    {
        private readonly IServiceProvider _provider;

        public GenericRepositoryFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IGenericRepositoryAsync<TEntity> Create(IContextProvider cp, DbContext? context = null)
        {
            var mode = cp._AppState.ConnectionMode?.ToLowerInvariant();



            if (mode == "api")
            {
                var httpClientFactory = _provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(cp._AppState.ApiName);

                var resource = typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryAsync<TEntity>(httpClient, cp, resource);
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
                            return new GenericRepositoryEFAsync<TEntity, SqlServerDbContext>(sqlServerDbContext);
                        }

                    case "sqlite":
                        {
                            var sqLiteDbContext = context as SqLiteDbContext;
                            return new GenericRepositoryEFAsync<TEntity, SqLiteDbContext>(sqLiteDbContext);
                        }

                    case "inmemory":
                        {
                            var inMemoryDbContext = context as InMemoryDbContext;
                            return new GenericRepositoryEFAsync<TEntity, InMemoryDbContext> (inMemoryDbContext);
                        }

                    default:
                        throw new NotSupportedException($"Contexto '{cp._AppState.DbKey}' no soportado.");
                }
            }

            throw new NotSupportedException($"Tipo de acceso '{cp._AppState.ConnectionMode}' no soportado.");
        }
    }


}
