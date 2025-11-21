using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity, TContext>, IGenericRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
    {
        public TContext Context { get; }
        public DbSet<TEntity> Set;

        public GenericRepository(TContext context)
        {
            Context = context;
            Set = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await Set.ToListAsync();

        public async Task<TEntity?> GetByIdAsync(object id) => await Set.FindAsync(id);

        public async Task AddAsync(TEntity entity) => await Set.AddAsync(entity);

        public void Update(TEntity entity) => Set.Update(entity);

        public void Remove(TEntity entity) => Set.Remove(entity);
    }


}
