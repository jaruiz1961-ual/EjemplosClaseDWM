using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorSeguridad2026.Data
{
    public class AppUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public AppUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (!(user.TenantId != null))
                identity.AddClaim(new Claim("TenantId", user.TenantId.ToString()));

            if (!string.IsNullOrEmpty(user.DbKey))
                identity.AddClaim(new Claim("DbKey", user.DbKey));

            if (!string.IsNullOrEmpty(user.AppState))
                identity.AddClaim(new Claim("AppState", user.AppState));

            return identity;
        }
    }

}
