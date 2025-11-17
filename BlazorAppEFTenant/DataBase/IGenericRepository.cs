
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public interface IGenericRepository<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        Task<TEntity?> GetByIdAsync(object id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        TContext Context { get; }
    }


}
