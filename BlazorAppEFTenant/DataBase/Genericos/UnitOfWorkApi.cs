using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class UnitOfWorkApi2 : IUnitOfWork
    {
        private readonly HttpClient _httpClient;
        private readonly string _contextKey;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWorkApi2(HttpClient httpClient, string contextKey)
        {
            _httpClient = httpClient;
            _contextKey = contextKey;
        }

        public DbContext Context => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {

            if (!_repositories.TryGetValue(typeof(TEntity), out var repo))
            {
                var resourceName = typeof(TEntity).Name.ToLower() + "s";
                repo = new GenericRepositoryApi<TEntity, DbContext>(_httpClient, _contextKey, resourceName);
                _repositories[typeof(TEntity)] = repo;
            }
            return (IGenericRepository<TEntity>)repo;
        }
        //public IGenericRepository<TEntity, TContext> GetRepository<TEntity, TContext>()
        //where TEntity : class
        //where TContext : DbContext
        //{
        //    // Aquí debes calcular el resourceName por convención, no pedirlo como parámetro externo.
        //    var resourceName = typeof(TEntity).Name.ToLower() + "s";
        //    // Construir el repo API sin parámetros extra
        //    return new GenericRepositoryApi<TEntity, TContext>(_httpClient, resourceName);
        //}

        public Task SaveChangesAsync() => Task.CompletedTask;

        Task<int> IUnitOfWork.SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
