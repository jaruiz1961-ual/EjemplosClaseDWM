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
        IGenericRepository<TEntity, TContext> Create<TEntity, TContext>(string tipoContexto, string resourceName = null)
            where TEntity : class
            where TContext : DbContext;
    }
}
