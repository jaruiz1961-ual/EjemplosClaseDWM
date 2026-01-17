using BlazorSeguridad2026.Base.Modelo;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BibliotecaContextosDb.TiposBase
{
    public static class Permissions
    {
        public const string UsersView = "Users.View";
        public const string UsersEdit = "Users.Edit";
        public const string RolesView = "Roles.View";
        public const string RolesEdit = "Roles.Edit";
        // lo que necesites…
    }

    public static class CustomClaimTypes
    {
        public const string Permission = "Permission";
    }

    public class RolePermissionSeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolePermissionSeeder(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task RolesPermissionsAsync(string roleName)
        {
            if (roleName == "Admin")
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole(roleName);
                    await _roleManager.CreateAsync(role);
                }

                var existingClaims = await _roleManager.GetClaimsAsync(role);

                async Task EnsurePermission(string permission)
                {
                    if (!existingClaims.Any(c =>
                            c.Type == CustomClaimTypes.Permission &&
                            c.Value == permission))
                    {
                        await _roleManager.AddClaimAsync(
                            role,
                            new Claim(CustomClaimTypes.Permission, permission));
                    }
                }

                await EnsurePermission(Permissions.UsersView);
                await EnsurePermission(Permissions.UsersEdit);
                await EnsurePermission(Permissions.RolesView);
                await EnsurePermission(Permissions.RolesEdit);
            }
        }
        
    }


}
