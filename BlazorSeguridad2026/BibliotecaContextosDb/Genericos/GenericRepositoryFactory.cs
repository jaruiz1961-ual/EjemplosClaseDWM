
using BibliotecaContextosDb.Genericos;
using BlazorSeguridad2026.Base.Contextos;
using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;





namespace BlazorSeguridad2026.Base.Genericos
{


    public interface IGenericRepositoryFactoryAsync<TEntity>
    where TEntity : class, IEntity
    {
        IGenericRepositoryAsync<TEntity> Create(IContextProvider cp, DbContext? context);
    }


    public class GenericRepositoryFactory<TEntity> : IGenericRepositoryFactoryAsync<TEntity>
     where TEntity : class, IEntity
    {
        private readonly IServiceProvider _provider;
        private readonly IReadOnlyDictionary<string, IRepositoryContextStrategy> _strategies;

        public GenericRepositoryFactory(
            IServiceProvider provider,
            IEnumerable<IRepositoryContextStrategy> strategies)
        {
            _provider = provider;
            _strategies = strategies.ToDictionary(s => s.Key.ToLowerInvariant());
        }

        public IGenericRepositoryAsync<TEntity> Create(IContextProvider cp, DbContext? context)
        {
            State? estado = cp.GetState();

            var mode = (estado.ConnectionMode??"Ef").ToLowerInvariant();

            if (mode == "api" || context is null)
            {
                var httpClientFactory = _provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(estado.ApiName);
                var resource = typeof(TEntity).Name.ToLower() + "s";
                return new GenericRepositoryAsync<TEntity>(httpClient, cp, resource);
            }

            if (mode == "ef")
            {
                if (context is null)
                    throw new ArgumentNullException(nameof(context), "Para EF se requiere un DbContext.");

                var key = estado.DbKey?.ToLowerInvariant() ?? string.Empty;

                if (!_strategies.TryGetValue(key, out var strategy))
                    throw new NotSupportedException($"Contexto '{estado.DbKey}' no soportado.");

                return strategy.Create<TEntity>(context, cp);
            }

            throw new NotSupportedException($"Tipo de acceso '{estado.ConnectionMode}' no soportado.");
        }
    }


}
