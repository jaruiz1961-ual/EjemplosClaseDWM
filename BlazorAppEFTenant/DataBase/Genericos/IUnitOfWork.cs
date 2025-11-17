using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public interface IUnitOfWork: IDisposable 
    {
     
        IUnitOfWork Create(string contextoKey);
        IGenericRepository<TEntity,DbContext>
            GetRepository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
    }

}
