using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
    where TContext : DbContext
    {
        private readonly Dictionary<Type, object> _repositories = new();

        public TContext Context { get; }
        DbContext IUnitOfWork.Context => Context;

        string _tipoContexto;
        IGenericRepository<Entidad, TContext> repo = null;
        private  IGenericRepositoryFactory _repositoryFactory;
        bool _isApi;

        public UnitOfWork(TContext context, ITenantProvider tenant)
        {
            Context = context;
            
        }
        public UnitOfWork(TContext context, ITenantProvider tenant, IGenericRepositoryFactory factory, string tipoContexto, bool isApi )
        {
            Context = context;
            _repositoryFactory = factory;
            _tipoContexto = tipoContexto;
            _isApi = isApi;
        }

        public IGenericRepository<TEntity, TContext> GetRepository<TEntity>()
            where TEntity : class
        {
            var type = typeof(TEntity);

            if (!_repositories.TryGetValue(type, out var repo))
            {
                 repo = _repositoryFactory.Create<TEntity, TContext>(_tipoContexto,Context,_isApi);
                    _repositories[type] = repo;

            }

            return (IGenericRepository<TEntity, TContext>)repo;
        }

       
        IGenericRepository<TEntity> IUnitOfWork.GetRepository<TEntity>()
            where TEntity : class
            => GetRepository<TEntity>();   // aquí ya compila, porque el return es IGenericRepository<TEntity, TContext> : IGenericRepository<TEntity>

        public Task<int> SaveChangesAsync() => Context?.SaveChangesAsync();

        public void Dispose() => Context?.Dispose();
    }


}
