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

            //if (entity != null && entity.TenantId == _tenantProvider.TenantId)
            //    return entity;
            //return null;
        }
        public async Task AddAsync(T data)
        {
            //data.TenantId = _tenantProvider.TenantId;
             var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            await repo.AddAsync(data);
            await uow.SaveChangesAsync();
        }
        public async Task UpdateAsync(T data)
        {
            //if (data.TenantId != _tenantProvider.TenantId)
            //    throw new UnauthorizedAccessException("No se puede editar una entidad de otro tenant.");
             var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            repo.Update(data);
            await uow.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
             var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var entity = await repo.GetByIdAsync(id);
            if (entity != null)// && entity.TenantId == _tenantProvider.TenantId)
            {
                repo.Remove(entity);
                await uow.SaveChangesAsync();
            }
        }
    }
    
}
