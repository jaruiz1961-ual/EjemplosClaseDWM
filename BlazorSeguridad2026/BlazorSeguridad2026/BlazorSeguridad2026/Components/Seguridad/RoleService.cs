using System.Net.NetworkInformation;

namespace BlazorSeguridad2026.Components.Seguridad
{
    using BlazorSeguridad2026.Data;
    // Services/IRoleService.cs
    using Microsoft.AspNetCore.Identity;

    public interface IRoleService
    {
        Task<List<ApplicationRole>> GetAllAsync();
        Task<IdentityResult> CreateAsync(string name, int? tenantId, string DbKey);
        Task<ApplicationRole?> GetByIdAsync(int id);
        Task<IdentityResult> UpdateRoleAsync(int id, string newName,int? tenantId, string DbKey);
        Task<IdentityResult> DeleteAsync(int id);
    }

    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<List<ApplicationRole>> GetAllAsync() =>
            Task.FromResult(_roleManager.Roles.ToList());

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

        public Task<ApplicationRole?> GetByIdAsync(int id) =>
            Task.FromResult(_roleManager.Roles.FirstOrDefault(r => r.Id == id));

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
