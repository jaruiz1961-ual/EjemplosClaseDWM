using DataBase.Genericos;
using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Servicios
{
    public class GenericDataService<T> : IGenericDataService<T> where T : class, ITenantEntity, IEntity
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITenantProvider _tenantProvider;
        private readonly IContextKeyProvider _contextProvider;
        private readonly string _apiName;
        public GenericDataService(IContextKeyProvider contextKeyProvider, IUnitOfWorkFactory uowFactory, ITenantProvider tenantProvider)
        {
            _contextProvider = contextKeyProvider;
            _tenantProvider = tenantProvider;
            _unitOfWorkFactory = uowFactory;
       

        }

 
        public async Task<List<T>> GetAllAsync()
        {
            var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var allEntities = await repo.GetAllAsync();
            return allEntities.Where(e => e.TenantId == _tenantProvider.CurrentTenantId).ToList();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
             var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            var entity = await repo.GetByIdAsync(id);
            if (entity != null && entity.TenantId == _tenantProvider.CurrentTenantId)
                return entity;
            return null;
        }
        public async Task AddAsync(T data)
        {
            data.TenantId = _tenantProvider.CurrentTenantId;
             var uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<T>();
            await repo.AddAsync(data);
            await uow.SaveChangesAsync();
        }
        public async Task UpdateAsync(T data)
        {
            if (data.TenantId != _tenantProvider.CurrentTenantId)
                throw new UnauthorizedAccessException("No se puede editar una entidad de otro tenant.");
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
            if (entity != null && entity.TenantId == _tenantProvider.CurrentTenantId)
            {
                repo.Remove(entity);
                await uow.SaveChangesAsync();
            }
        }
    }
    
}
