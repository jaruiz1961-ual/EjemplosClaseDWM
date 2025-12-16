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
    public interface IGenericDataService<T> where T : class, ITenantEntity
    {
        Task<IEnumerable<T>> ObtenerTodosAsync(bool reload);
        Task<T?> ObtenerPorIdAsync(int id, bool reload);
        Task<IEnumerable<T>> ObtenerFiltradosCadenaAsync(string filtro, bool reload);
        Task<IEnumerable<T>> ObtenerFiltradosExpresionAsync(Expression<Func<T, bool>> predicate, bool reload);


        Task<T?> AñadirAsync(T data, bool reload);
        Task<T?> ActualizarAsync(T data, bool reload);
        Task<T?> EliminarAsync(int id, bool reload);
    }
    public class GenericDataService<T> : IGenericDataService<T> where T : class, ITenantEntity
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private IUnitOfWorkAsync uow;

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
         public async Task<IEnumerable<T>> ObtenerTodosAsync(bool reload)
        {
            if (uow == null)
             uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>(reload);
            var allEntities = await repo.GetAllAsync(reload);
            return allEntities.ToList();
        }
        public async Task<IEnumerable<T>> ObtenerFiltradosCadenaAsync(string filtro, bool reload)
        {
            var predicate = DynamicExpressionParser.ParseLambda<T, bool>(
                ParsingConfig.Default,false,filtro );
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>(reload);
            var filterEntities = await repo.GetFilterAsync(predicate,reload);
            return filterEntities.ToList();
        }
        public async Task<IEnumerable<T>> ObtenerFiltradosExpresionAsync(Expression<Func<T, bool>> predicate, bool reload)
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>(reload);
            var filterEntities = await repo.GetFilterAsync(predicate,reload);
            return filterEntities.ToList();         
        }
        public async Task<T?> ObtenerPorIdAsync(int id, bool reload)
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>(reload);
            var entity = await repo.GetByIdAsync(id,reload);
            return entity;
        }
        public async Task<T?> AñadirAsync(T data, bool reload)
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>(reload);
            var addedData = await repo.Add(data,reload);
            var num = await uow.SaveChangesAsync();
            if (num>0)  return addedData;
            else return null;
        }
        public async Task<T?> ActualizarAsync(T data, bool reload)
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>(reload);
            var updatedData= await repo.Update(data,reload);
            var num = await uow.SaveChangesAsync();
            if (num > 0) return updatedData;
            else return null;
        }
        public async Task<T?> EliminarAsync(int id, bool reload)
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>(reload);
            var entity = await repo.GetByIdAsync(id,reload);
            if (entity != null)
            {
                var deletedData = await repo.Remove(entity,reload);
                var num= await uow.SaveChangesAsync();
                if (num > 0) return deletedData;
            }
            return null;
        }
    }
    
}
