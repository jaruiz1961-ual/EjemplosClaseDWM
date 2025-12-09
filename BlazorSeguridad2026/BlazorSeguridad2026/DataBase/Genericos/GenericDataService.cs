using DataBase.Genericos;
using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Servicios
{
    public class GenericDataService<T> : IGenericDataService<T> where T : class, ITenantEntity, IEntity
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        private readonly IContextProvider _contextProvider;
        private readonly string _apiName;
        public GenericDataService(IContextProvider contextKeyProvider, IUnitOfWorkFactory uowFactory)
        {
            _contextProvider = contextKeyProvider;         
            _unitOfWorkFactory = uowFactory;
       

        }

 
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var allEntities = await repo.GetAllAsync();
            return allEntities.ToList();
            //return allEntities.Where(e => e.TenantId == _tenantProvider.TenantId).ToList();
        }



        public async Task<IEnumerable<T>> GetFilterAsync(string filtro)
        {
            var predicate = DynamicExpressionParser.ParseLambda<T, bool>(
                ParsingConfig.Default,false,filtro );

            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var filterEntities = await repo.GetFilterAsync(predicate);
            return filterEntities.ToList();
            //return allEntities.Where(e => e.TenantId == _tenantProvider.TenantId).ToList();
        }
        public async Task<IEnumerable<T>> GetFilterAsync(Expression<Func<T, bool>> predicate)
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var filterEntities = await repo.GetFilterAsync(predicate);
            return filterEntities.ToList();
            //return allEntities.Where(e => e.TenantId == _tenantProvider.TenantId).ToList();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
             var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var entity = await repo.GetByIdAsync(id);
            return entity;
        }
        public async Task<T?> AddAsync(T data)
        {

             var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var addedData = await repo.Add(data);
            var num = await uow.SaveChangesAsync();
            if (num>0)  return addedData;
            else return null;
        }
        public async Task<T?> UpdateAsync(T data)
        {
              var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var updatedData= await repo.Update(data);
            var num = await uow.SaveChangesAsync();
            if (num > 0) return updatedData;
            else return null;
        }
        public async Task<T?> DeleteAsync(int id)
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
