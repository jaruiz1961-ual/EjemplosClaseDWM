using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext
    {
        public TContext Context { get; }

        public GenericRepository(TContext context)
        {
            Context = context;
        }

        public async Task<TEntity?> GetByIdAsync(object id) =>
            await Context.Set<TEntity>().FindAsync(id);

        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await Context.Set<TEntity>().ToListAsync();

        public async Task AddAsync(TEntity entity) =>
            await Context.Set<TEntity>().AddAsync(entity);

        public void Update(TEntity entity) =>
            Context.Set<TEntity>().Update(entity);

        public void Remove(TEntity entity) =>
            Context.Set<TEntity>().Remove(entity);
    }


}
