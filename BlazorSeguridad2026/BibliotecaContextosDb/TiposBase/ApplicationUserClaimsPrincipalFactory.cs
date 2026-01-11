using BlazorSeguridad2026.Base.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BibliotecaContextosDb.TiposBase
{
    public class ApplicationUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            // La base ya:
            //  - Añade Name, NameIdentifier, etc.
            //  - Añade roles del usuario
            //  - Añade RoleClaims (claims de cada rol)
            var identity = await base.GenerateClaimsAsync(user);

            // Aquí puedes añadir claims adicionales (TenantId, etc.)
            if (user.TenantId.HasValue)
            {
                identity.AddClaim(
                    new Claim("TenantId", user.TenantId.Value.ToString()));
            };
            if (user.DbKey != null)
            {
                identity.AddClaim(
                    new Claim("DbKey", user.DbKey.ToString()));
            };
            if (!string.IsNullOrEmpty(user.AppState))
            {
                identity.AddClaim(
                    new Claim("AppState", user.AppState));
            };



            return identity;
        }
    }

}
