using BlazorSeguridad2026.Data;
using Microsoft.AspNetCore.Identity;
using Shares.Genericos;
using Shares.Seguridad;
using System.Net.NetworkInformation;

namespace BlazorSeguridad2026.Components.Seguridad
{

    public class RoleServiceMio : IRoleService
    {
        IContextProvider _contextProvider;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private IUnitOfWorkAsync uow;
        bool reload = true;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleServiceMio(RoleManager<ApplicationRole> roleManager, IContextProvider contextKeyProvider, IUnitOfWorkFactory uowFactory)
        {
            _contextProvider = contextKeyProvider;
            _unitOfWorkFactory = uowFactory;
            
            _roleManager = roleManager;
        }

        public async Task<List<ApplicationRole>> GetAllAsync()
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);

            var repo = uow.GetRepository<ApplicationRole>(reload);

            var allEntities = await repo.GetAllAsync(reload); // IEnumerable<ApplicationUser> o similar 
            var lista = allEntities.ToList();
            return lista;
        }



        public async Task<IdentityResult> CreateAsync(string name, int? tenantId, string DbKey)
        {
            var role = new ApplicationRole
            {
                Name = name.Trim(),
                NormalizedName = name.Trim().ToUpperInvariant(),
                TenantId = tenantId,
                DbKey = DbKey
            };

            return await _roleManager.CreateAsync(role);
        }

        public async Task<ApplicationRole?> GetByIdAsync(int id)
        {
            if (uow == null)
                uow = _unitOfWorkFactory.Create(_contextProvider);
            var repo = uow.GetRepository<ApplicationRole>(reload);

        var entity = await repo.GetByIdAsync(id, reload);

            return entity;
            
        }

        public async Task<IdentityResult> UpdateRoleAsync(int id, string newName, int? tenantId, string DbKey)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            // O mejor: _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id)
            if (role is null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            role.Name = newName.Trim();
            role.NormalizedName = role.Name.ToUpperInvariant();
            role.TenantId = tenantId;
            role.DbKey = DbKey;
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role is null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            return await _roleManager.DeleteAsync(role);
        }
    }
}
