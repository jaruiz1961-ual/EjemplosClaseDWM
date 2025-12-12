using Shares.Genericos;
using Shares.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Shares.Seguridad;

namespace Shares.Servicios
{
    public interface IGenericDataService<T> where T : class, ITenantEntity, IEntity
    {
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<T?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<T>> ObtenerFiltradosCadenaAsync(string filtro);
        Task<IEnumerable<T>> ObtenerFiltradosExpresionAsync(Expression<Func<T, bool>> predicate);


        Task<T?> AñadirAsync(T data);
        Task<T?> ActualizarAsync(T data);
        Task<T?> EliminarAsync(int id);
    }
    public class GenericDataService<T> : IGenericDataService<T> where T : class, ITenantEntity, IEntity
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        private readonly IContextProvider _contextProvider;
        private readonly string? _apiName;
        public GenericDataService(IContextProvider contextKeyProvider, IUnitOfWorkFactory uowFactory, bool isApi=false)
        {
            _contextProvider = contextKeyProvider;    
            if (isApi)
            {
                _contextProvider._AppState.ConnectionMode = "Api";
            }
            _unitOfWorkFactory = uowFactory;       
        }
         public async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var allEntities = await repo.GetAllAsync();
            return allEntities.ToList();
        }
        public async Task<IEnumerable<T>> ObtenerFiltradosCadenaAsync(string filtro)
        {
            var predicate = DynamicExpressionParser.ParseLambda<T, bool>(
                ParsingConfig.Default,false,filtro );

            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var filterEntities = await repo.GetFilterAsync(predicate);
            return filterEntities.ToList();
        }
        public async Task<IEnumerable<T>> ObtenerFiltradosExpresionAsync(Expression<Func<T, bool>> predicate)
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var filterEntities = await repo.GetFilterAsync(predicate);
            return filterEntities.ToList();         
        }
        public async Task<T?> ObtenerPorIdAsync(int id)
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var entity = await repo.GetByIdAsync(id);
            return entity;
        }
        public async Task<T?> AñadirAsync(T data)
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var addedData = await repo.Add(data);
            var num = await uow.SaveChangesAsync();
            if (num>0)  return addedData;
            else return null;
        }
        public async Task<T?> ActualizarAsync(T data)
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var updatedData= await repo.Update(data);
            var num = await uow.SaveChangesAsync();
            if (num > 0) return updatedData;
            else return null;
        }
        public async Task<T?> EliminarAsync(int id)
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var entity = await repo.GetByIdAsync(id);
            if (entity != null)
            {
                var deletedData = await repo.Remove(entity);
                var num= await uow.SaveChangesAsync();
                if (num > 0) return deletedData;
            }
            return null;
        }
    }
    
}
