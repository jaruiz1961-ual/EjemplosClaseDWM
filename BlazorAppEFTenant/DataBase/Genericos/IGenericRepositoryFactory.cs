using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public interface IGenericRepositoryFactory
    {

        public IGenericRepository<TEntity, TContext> Create<TEntity, TContext>(string dbContextKey, bool isApi, string apiResourceName = null)
           where TEntity : class
           where TContext : DbContext;
    }
}
