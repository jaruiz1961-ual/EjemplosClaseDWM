using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public interface IGenericRepositoryEF<TEntity,TContext>: IGenericRepository<TEntity>
        where TEntity : class

    {
        TContext Context { get; }
    }
    public interface IGenericRepository<TEntity>
    where TEntity : class
    {
        // Mismos métodos (o un subconjunto), pero sin TContext
        Task<TEntity?> GetByIdAsync(object id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }



}
