using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using BlazorSeguridad2026.Data;
using Shares.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Shares.Genericos
{
    public interface IUnitOfWorkEfAsync : IUnitOfWorkAsync,IDisposable 
    {
        DbContext Context { get; }
        void BeginTransactionAsync();
        void EndTransactionAsync();
        void RollbackAsync();

    }
    public class UnitOfWorkEfAsync<TContext> : UnitOfWorkAsync,IUnitOfWorkEfAsync
    where TContext : DbContext
    {
        public DbContext Context { get; }
        private readonly IContextProvider _cp;
        private readonly IServiceProvider _provider;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWorkEfAsync(TContext context, IServiceProvider provider, IContextProvider cp):base(cp, provider)
        {
            Context = context;
            _cp = cp;
            _provider = provider;

        }

        private bool _disposed;
        private string _errorMessage = string.Empty;
        private Task<IDbContextTransaction>? _dbContextTransaction;

        public new IGenericRepositoryAsync<TEntity> GetRepository<TEntity>(bool reload)  where TEntity : class, IEntity
        {
            var type = typeof(TEntity);
            //comprobamos si el repositorio no existe en el diccionario o si es necesario recargarlo
            if (reload || !_repositories.TryGetValue(type, out var repo))
            {
                // Resolvemos la factoría específica para TEntity
                var factory = _provider.GetRequiredService<IGenericRepositoryFactoryAsync<TEntity>>();
                // Para API se crea el repositorio pasando DbContext null
                repo = factory.Create(_cp, Context);
                _repositories[type] = repo;
            }

            return (IGenericRepositoryAsync<TEntity>)repo!;
        }
        public new async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }
        public void BeginTransactionAsync()
        {
            _dbContextTransaction = Context.Database.BeginTransactionAsync();
        }
        public async void EndTransactionAsync()
        {
            if (_dbContextTransaction != null)
            {
                var transaccion = await _dbContextTransaction;
                await transaccion.CommitAsync();
            }
        }

        public async void RollbackAsync()
        {
            if (_dbContextTransaction != null)
            {
                var transaccion = await _dbContextTransaction;
                await transaccion.RollbackAsync();
                _dbContextTransaction.Dispose();
            }
        }
        
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            _disposed = true;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (Context != null)
                {
                    Context.Dispose();
                }
                _disposed = true;
            }
        }
    }


}
