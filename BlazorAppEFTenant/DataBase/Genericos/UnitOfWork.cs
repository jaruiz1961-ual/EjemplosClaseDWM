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
        public TContext Context { get; }
        TContext IUnitOfWork<TContext>.Context { get => Context; set => throw new NotImplementedException(); }

        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(TContext context)
        {
            Context = context;
        }

        public IGenericRepository<TEntity, TContext> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                var repo = new GenericRepository<TEntity, TContext>(Context);
                _repositories[type] = repo;
            }
            return (IGenericRepository<TEntity, TContext>)_repositories[type];
        }

        public int SaveChanges() => Context.SaveChanges();

        public async Task<int> SaveChangesAsync() =>
            await Context.SaveChangesAsync();

        public void Dispose() => Context.Dispose();


    }
}
