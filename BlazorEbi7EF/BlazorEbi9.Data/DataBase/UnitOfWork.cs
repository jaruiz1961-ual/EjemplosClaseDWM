using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace BlazorEbi9.Data.DataBase
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext DataBaseContext { get; set; }
        DbSet<T> Set<T>() where T : class;
        EntityEntry<T> Entry<T>(T entity) where T : class;
        void BeginTransaction();
        void EndTransaction();
        void Rollback();
        int SaveChanges();
    }

    public class UnitOfWork<T> : IUnitOfWork where T : DbContext, IDisposable, new()
    {
        private bool _disposed;
        private string _errorMessage = string.Empty;
        private IDbContextTransaction? _dbContextTransaction;
        public DbContext DataBaseContext { get; set; }
        public UnitOfWork(T context)
        {
            DataBaseContext = context;
            DataBaseContext.Database.Migrate();
            DataBaseContext.Database.EnsureCreated();
        }
        public int SaveChanges()
        {
            return DataBaseContext.SaveChanges();
        }
        public void BeginTransaction()
        {
            _dbContextTransaction = DataBaseContext.Database.BeginTransaction();
        }
        public void EndTransaction()
        {
            if (_dbContextTransaction != null)
            {
                _dbContextTransaction.Commit();

            }
        }


        public void Rollback()
        {
            if (_dbContextTransaction != null)
            {
                _dbContextTransaction.RollbackAsync();
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