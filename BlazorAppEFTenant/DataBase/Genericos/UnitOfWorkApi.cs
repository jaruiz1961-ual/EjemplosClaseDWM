using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class UnitOfWorkApi : IUnitOfWork
    {
        private readonly IContextProvider _cp;
        private readonly IServiceProvider _provider;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWorkApi(IContextProvider cp, IServiceProvider provider)
        {
            _cp = cp;
            _provider = provider;
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class,IEntity
        {
            var type = typeof(TEntity);

            //if (!_repositories.TryGetValue(type, out var repo))
            //{
                // Resolvemos la factoría específica para TEntity
                var factory = _provider.GetRequiredService<IGenericRepositoryFactory<TEntity>>();
                // Para API no pasas DbContext
                var repo = factory.Create(_cp);
                _repositories[type] = repo;
            //}

            return (IGenericRepository<TEntity>)repo!;
        }

        IGenericRepository<TEntity> IUnitOfWork.GetRepository<TEntity>()
            where TEntity : class
            => GetRepository<TEntity>();

        public Task<int> SaveChangesAsync() => Task.FromResult(0);

        public void Dispose() { }
    }


}
