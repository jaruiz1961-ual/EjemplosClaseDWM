using System.Net.NetworkInformation;

namespace BlazorSeguridad2026.Components.Seguridad
{
    using BlazorSeguridad2026.Data;
    // Services/IRoleService.cs
    using Microsoft.AspNetCore.Identity;
    using Shares.Seguridad;

    public interface IRoleService
    {
        Task<List<ApplicationRole>> GetAllAsync();
        Task<IdentityResult> CreateAsync(string name);
        Task<ApplicationRole?> GetByIdAsync(int id);
        Task<IdentityResult> UpdateRoleAsync(int id, string newName);
        Task<IdentityResult> DeleteAsync(int id);
    }

    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private int? _tenantId;
        private string _dbKey;

        public RoleService(RoleManager<ApplicationRole> roleManager, IContextProvider contextKeyProvider)
        {
            _roleManager = roleManager;
            _tenantId= contextKeyProvider._AppState.TenantId;
            _dbKey= contextKeyProvider._AppState.DbKey;
        }

        public Task<List<ApplicationRole>> GetAllAsync() =>
            Task.FromResult(_roleManager.Roles.ToList());

        public async Task<IdentityResult> CreateAsync(string name)
        {
            var role = new ApplicationRole
            {
                Name = name.Trim(),
                NormalizedName = name.Trim().ToUpperInvariant(),
                TenantId = _tenantId,
                DbKey = _dbKey
            };

            return await _roleManager.CreateAsync(role);
        }

        public Task<ApplicationRole?> GetByIdAsync(int id) =>
            Task.FromResult(_roleManager.Roles.FirstOrDefault(r => r.Id == id));

        public async Task<IdentityResult> UpdateRoleAsync(int id, string newName)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            // O mejor: _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id)
            if (role is null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            role.Name = newName.Trim();
            role.NormalizedName = role.Name.ToUpperInvariant();
            role.TenantId = _tenantId;
            role.DbKey = _dbKey;
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
