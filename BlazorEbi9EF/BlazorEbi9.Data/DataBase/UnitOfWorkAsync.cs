using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

using System;

namespace BlazorEbi9.Data.DataBase
{
    public interface IUnitOfWorkAsync : IDisposable
    {
        DbContext DataBaseContext { get; set; }
        DbSet<T> Set<T>() where T : class;
        EntityEntry<T> Entry<T>(T entity) where T : class;
        void BeginTransactionAsync();
        void EndTransactionAsync();
        void RollbackAsync();
        Task<int> SaveChangesAsync();
    }

    public class UnitOfWorkAsync<T> : IUnitOfWorkAsync where T : DbContext, IDisposable
    {
        private bool _disposed;
        private string _errorMessage = string.Empty;
        private Task<IDbContextTransaction>? _dbContextTransaction;
        public DbContext? DataBaseContext { get; set; }

        public UnitOfWorkAsync() 
        {
        }
        public UnitOfWorkAsync(T context)
        {
            DataBaseContext = context;
            try
            {
                DataBaseContext.Database.Migrate();
                DataBaseContext.Database.EnsureCreated();
            }
            catch { }
        }
        public async Task<int> SaveChangesAsync()
        {
            return await DataBaseContext.SaveChangesAsync();
        }
        public void BeginTransactionAsync()
        {
            _dbContextTransaction = DataBaseContext.Database.BeginTransactionAsync();
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
        public DbSet<E> Set<E>() where E : class
        {
            return DataBaseContext.Set<E>();
        }

        public EntityEntry<E> Entry<E>(E entity) where E : class
        {
            return DataBaseContext.Entry(entity);
        }
        public void Dispose()
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
                if (DataBaseContext != null)
                {
                    DataBaseContext.Dispose();
                }
                _disposed = true;
            }
        }
    }

}