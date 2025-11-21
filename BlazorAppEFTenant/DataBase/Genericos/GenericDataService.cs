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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        public GenericDataService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
        }
        public async Task<List<T>> GetAllAsync()
        {
            var repo = _unitOfWork.GetRepository<T>();
            var allEntities = await repo.GetAllAsync();
            return allEntities.Where(e => e.TenantId == _tenantProvider.CurrentTenantId).ToList();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<T>();
            var entity = await repo.GetByIdAsync(id);
            if (entity != null && entity.TenantId == _tenantProvider.CurrentTenantId)
                return entity;
            return null;
        }
        public async Task AddAsync(T data)
        {
            data.TenantId = _tenantProvider.CurrentTenantId;
            var repo = _unitOfWork.GetRepository<T>();
            await repo.AddAsync(data);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(T data)
        {
            if (data.TenantId != _tenantProvider.CurrentTenantId)
                throw new UnauthorizedAccessException("No se puede editar una entidad de otro tenant.");
            var repo = _unitOfWork.GetRepository<T>();
            repo.Update(data);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<T>();
            var entity = await repo.GetByIdAsync(id);
            if (entity != null && entity.TenantId == _tenantProvider.CurrentTenantId)
            {
                repo.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
    
}
