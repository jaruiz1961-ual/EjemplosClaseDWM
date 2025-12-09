using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Genericos
{
    public interface IGenericRepositoryFactory<TEntity> where TEntity : class,IEntity
    {
        public IGenericRepository<TEntity> Create(IContextProvider ic, DbContext context = null);

    }
}
