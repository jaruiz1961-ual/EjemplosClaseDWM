using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class GenericDataService<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext
    {
        private readonly IUnitOfWork<TContext> _unitOfWork;
        private readonly ITenantProvider _tenantProvider;

        public GenericDataService(IUnitOfWork<TContext> unitOfWork, ITenantProvider tenantProvider)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var repo = _unitOfWork.GetRepository<TEntity>();
            return await repo.GetAllAsync();
        }

        public async Task<TEntity?> GetByIdAsync(object id)
        {
            var repo = _unitOfWork.GetRepository<TEntity>();
            return await repo.GetByIdAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity is ITenantEntity tenantEntity)
            {
                tenantEntity.TenantId = _tenantProvider.CurrentTenantId;
            }
            var repo = _unitOfWork.GetRepository<TEntity>();
            await repo.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var repo = _unitOfWork.GetRepository<TEntity>();
            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAsync(object id)
        {
            var repo = _unitOfWork.GetRepository<TEntity>();
            var entity = await repo.GetByIdAsync(id);
            if (entity != null)
            {
                repo.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }


}
