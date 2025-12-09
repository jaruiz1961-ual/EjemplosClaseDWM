using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace Shares.Genericos
{
    public class UnitOfWorkEf<TContext> : IUnitOfWork
    where TContext : DbContext
    {
        private readonly Dictionary<Type, object> _repositories = new();

        public TContext Context { get; }


        IGenericRepositoryEF<Entidad,TContext> repo = null;
        private  IGenericRepositoryFactory<Entidad> _repositoryFactory;
        IContextProvider _cp;
        IServiceProvider _provider;

        //public UnitOfWork(TContext context, ITenantProvider tenant)
        //{
        //    Context = context;

        //}
        public UnitOfWorkEf(TContext context, IServiceProvider provider, IContextProvider cp)
        {
            Context = context;
            _cp = cp;
            _provider = provider;

        }

        public IGenericRepository<TEntity> GetRepository<TEntity>()
    where TEntity : class,IEntity
        {
            var type = typeof(TEntity);
            var _repositoryFactory = _provider.GetRequiredService<IGenericRepositoryFactory<TEntity>>();

            var repo = _repositoryFactory.Create(_cp,Context);
            _repositories[type] = repo; // Siempre actualiza (o reemplaza)

            return (IGenericRepositoryEF<TEntity,TContext>)repo!;
        }

        public Task<int> SaveChangesAsync() => Context?.SaveChangesAsync();

        public void Dispose() => Context?.Dispose();
    }


}
