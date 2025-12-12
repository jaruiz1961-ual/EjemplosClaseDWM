using BlazorSeguridad2026.Data;
using Microsoft.AspNetCore.Identity;

namespace BlazorSeguridad2026.Components.Account
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);

        Task<IdentityResult> CreateAsync(string email, string password);
        Task<IdentityResult> UpdateEmailAsync(string id, string newEmail);
        Task<IdentityResult> DeleteAsync(string id);

        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IdentityResult> SetUserRolesAsync(string userId, IEnumerable<string> roles);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<List<ApplicationUser>> GetAllAsync() =>
            Task.FromResult(_userManager.Users.ToList());

        public Task<ApplicationUser?> GetByIdAsync(string id) =>
            Task.FromResult(_userManager.Users.FirstOrDefault(u => u.Id == id));

        public async Task<IdentityResult> CreateAsync(string email, string password)
        {
            var user = new ApplicationUser { UserName = email, Email = email };
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateEmailAsync(string id, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            user.Email = newEmail;
            user.UserName = newEmail;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user is null
                ? Array.Empty<string>()
                : await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> SetUserRolesAsync(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
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

