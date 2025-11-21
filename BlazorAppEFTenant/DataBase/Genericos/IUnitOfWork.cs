using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        new TContext Context { get; }
        new IGenericRepository<TEntity, TContext> GetRepository<TEntity>() where TEntity : class;
    }


}
