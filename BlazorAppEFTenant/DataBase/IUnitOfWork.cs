using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public interface IUnitOfWork<TContext>: IDisposable where TContext: DbContext
    {
        TContext Context { get; }
        IGenericRepository<TEntity,TContext>
            GetRepository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
    }

}
