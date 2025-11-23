using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public UnitOfWork(TContext context, ITenantProvider tenant)
        {
            Context = context;
            
        }

        public IGenericRepository<TEntity, TContext> GetRepository<TEntity>()
            where TEntity : class
        {
            var type = typeof(TEntity);

            if (!_repositories.TryGetValue(type, out var repo))
            {
                repo = new GenericRepository<TEntity, TContext>(Context);
                _repositories[type] = repo;
            }

            return (IGenericRepository<TEntity, TContext>)repo;
        }

        IGenericRepository<TEntity> IUnitOfWork.GetRepository<TEntity>()
            where TEntity : class
            => GetRepository<TEntity>();   // aquí ya compila, porque el return es IGenericRepository<TEntity, TContext> : IGenericRepository<TEntity>

        public Task<int> SaveChangesAsync() => Context.SaveChangesAsync();

        public void Dispose() => Context.Dispose();
    }


}
