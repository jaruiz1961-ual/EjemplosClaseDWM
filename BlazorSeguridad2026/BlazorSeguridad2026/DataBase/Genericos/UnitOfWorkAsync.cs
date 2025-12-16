using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shares.Modelo;
using Shares.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Genericos
{

    public interface IUnitOfWorkAsync : IDisposable
    {
        IGenericRepositoryAsync<TEntity> GetRepository<TEntity>(bool reload) where TEntity : class;
        Task<int> SaveChangesAsync();
    }

    public class UnitOfWorkAsync : IUnitOfWorkAsync
    {
        private readonly IContextProvider _cp;
        private readonly IServiceProvider _provider;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWorkAsync(IContextProvider cp, IServiceProvider provider)
        {
            _cp = cp;
            _provider = provider;
        }

        public virtual IGenericRepositoryAsync<TEntity> GetRepository<TEntity>(bool reload)
            where TEntity : class
        {
            var type = typeof(TEntity);
            //comprobamos si el repositorio no existe en el diccionario o si es necesario recargarlo
            if (reload || !_repositories.TryGetValue(type, out var repo))
            {
                // Resolvemos la factoría específica para TEntity
                var factory = _provider.GetRequiredService<IGenericRepositoryFactoryAsync<TEntity>>();
                // Para API se crea el repositorio pasando DbContext null
                 repo = factory.Create(_cp,null);
                _repositories[type] = repo;
            }

            return (IGenericRepositoryAsync<TEntity>)repo!;
        }


        public virtual Task<int> SaveChangesAsync() => Task.FromResult(0);

        public void Dispose() { }

     
    }


}
