using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Genericos
{
    public interface IGenericRepositoryEfAsync<TEntity, TContext> : IGenericRepositoryAsync<TEntity>
    where TEntity : class

    {
        TContext Context { get; }
    }
    public class GenericRepositoryEFAsync<TEntity, TContext> : IGenericRepositoryEfAsync<TEntity, TContext>, IGenericRepositoryAsync<TEntity>
    where TEntity : class
    where TContext : DbContext
    {
        public TContext Context { get; }
        public DbSet<TEntity> Set;

        public GenericRepositoryEFAsync(TContext context)
        {
            Context = context;
            Set = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await Set.ToListAsync<TEntity>();

        public async Task<IEnumerable<TEntity>> GetFilterAsync(Expression<Func<TEntity, bool>> predicate) => 
            await Set.Where(predicate).ToListAsync<TEntity>();
        
        public async Task<TEntity?> GetByIdAsync(object id) => await Set.FindAsync(id);

        public async Task<TEntity?> Add(TEntity entity)
        {
            await Set.AddAsync(entity);
            return entity;
        }

        public async  Task<TEntity?> Update(TEntity entity)
        {
            Set.Update(entity);
           return entity;
        }

        public async Task<TEntity?> Remove(TEntity entity)
        {
            Set.Remove(entity);
            return entity;
        }
    }


}
