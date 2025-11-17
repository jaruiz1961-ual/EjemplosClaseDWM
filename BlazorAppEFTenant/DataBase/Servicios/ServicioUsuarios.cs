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
    public class ServicioUsuarios <TContext> where TContext : DbContext
    {
        private readonly IUnitOfWork<TContext> _unitOfWork;
        private readonly ITenantProvider _tenantProvider;

        public ServicioUsuarios(IUnitOfWork<TContext> unitOfWork, ITenantProvider tenantProvider)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            var repo = _unitOfWork.GetRepository<Usuario>();
            var usuarios = await repo.GetAllAsync();
            // No es necesario filtrar manualmente por TenantId si tienes filtros globales en el DbContext,
            // pero puedes hacerlo aquí adicionalmente si lo deseas:
            return usuarios.Where(u => u.TenantId == _tenantProvider.CurrentTenantId).ToList();
        }
        public async Task<Usuario?> GetFirstAsync()
        {
            var result = _unitOfWork.Context.Set<Usuario>()
                .Where(u => u.TenantId == _tenantProvider.CurrentTenantId)
                .Take(1);

            return await result.FirstOrDefaultAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Usuario>();
            var user = await repo.GetByIdAsync(id);
            // Validación adicional opcional de tenant
            if (user != null && user.TenantId == _tenantProvider.CurrentTenantId)
                return user;
            return null;
        }

        public async Task AddAsync(Usuario usuario)
        {
            // Asignar TenantId antes de guardar para seguridad extra
            usuario.TenantId = _tenantProvider.CurrentTenantId;
            var repo = _unitOfWork.GetRepository<Usuario>();
            await repo.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            // Reforzar seguridad multi-tenant al actualizar
            if (usuario.TenantId != _tenantProvider.CurrentTenantId)
                throw new UnauthorizedAccessException("No se puede editar un usuario de otro tenant.");
            var repo = _unitOfWork.GetRepository<Usuario>();
            repo.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Usuario>();
            var user = await repo.GetByIdAsync(id);
            if (user != null && user.TenantId == _tenantProvider.CurrentTenantId)
            {
                repo.Remove(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }

}
