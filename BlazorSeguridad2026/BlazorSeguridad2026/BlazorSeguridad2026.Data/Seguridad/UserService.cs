using BlazorSeguridad2026.Data;
using Microsoft.AspNetCore.Identity;
using Shares.Seguridad;

namespace BlazorSeguridad2026.Components.Seguridad
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(int id);

        Task<IdentityResult> CreateAsync(string email, string password, int? TenantId, string KeyDb);
        Task<IdentityResult> UpdateUserAsync(int id, Action<ApplicationUser> updateAction);

        Task<IdentityResult> DeleteAsync(int id);

        Task<IList<string>> GetUserRolesAsync(int userId);
        Task<IdentityResult> SetUserRolesAsync(int userId, IEnumerable<string> roles);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private int? tenantId;

        public UserService(UserManager<ApplicationUser> userManager, IContextProvider contextKeyProvider)
        {
            _userManager = userManager;
            tenantId = contextKeyProvider._AppState.TenantId;
        }

        public Task<List<ApplicationUser>> GetAllAsync() =>
            Task.FromResult(_userManager.Users.ToList());

        public Task<ApplicationUser?> GetByIdAsync(int id) =>
            Task.FromResult(_userManager.Users.FirstOrDefault(u => u.Id == id));

        public async Task<IdentityResult> CreateAsync(string email, string password, int? tenantId, string keyDb)
        {
            var user = new ApplicationUser { UserName = email, Email = email, TenantId = tenantId, DbKey = keyDb};
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(int id, Action<ApplicationUser> updateAction)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            updateAction(user);
            return await _userManager.UpdateAsync(user);
        }
        

       

        public async Task<IdentityResult> DeleteAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IList<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user is null
                ? Array.Empty<string>()
                : await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> SetUserRolesAsync(int userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var currentRoles = await _userManager.GetRolesAsync(user);

            var toRemove = currentRoles.Except(roles).ToArray();
            var toAdd = roles.Except(currentRoles).ToArray();

            if (toRemove.Length > 0)
            {
                var r1 = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!r1.Succeeded) return r1;
            }

            if (toAdd.Length > 0)
            {
                var r2 = await _userManager.AddToRolesAsync(user, toAdd);
                if (!r2.Succeeded) return r2;
            }

            return IdentityResult.Success;
        }
    }
}

