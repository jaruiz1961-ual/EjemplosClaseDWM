namespace BlazorSeguridad2026.Components.Account
{
    // Services/IRoleService.cs
    using Microsoft.AspNetCore.Identity;

    public interface IRoleService
    {
        Task<List<IdentityRole>> GetAllAsync();
        Task<IdentityResult> CreateAsync(string name);
        Task<IdentityRole?> GetByIdAsync(string id);
        Task<IdentityResult> UpdateNameAsync(string id, string newName);
        Task<IdentityResult> DeleteAsync(string id);
    }

    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<List<IdentityRole>> GetAllAsync() =>
            Task.FromResult(_roleManager.Roles.ToList());

        public async Task<IdentityResult> CreateAsync(string name)
        {
            var role = new IdentityRole(name.Trim());
            return await _roleManager.CreateAsync(role);
        }

        public Task<IdentityRole?> GetByIdAsync(string id) =>
            Task.FromResult(_roleManager.Roles.FirstOrDefault(r => r.Id == id));

        public async Task<IdentityResult> UpdateNameAsync(string id, string newName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            role.Name = newName.Trim();
            role.NormalizedName = role.Name.ToUpperInvariant();
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            return await _roleManager.DeleteAsync(role);
        }
    }


}
