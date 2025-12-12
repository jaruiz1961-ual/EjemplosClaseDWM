using BlazorSeguridad2026.Data;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shares.Seguridad;
using Shares.SeguridadToken;
using Shares.Servicios;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace BlazorAppEFTenant.Components.EndPoints
{

    public static class LoginApiEndpoints
    {

        public static void LoginApis(this WebApplication app)
        {
            app.MapGet("/Logout", async (HttpContext context, string? returnUrl, IContextProvider ContextProvider) =>
            {
                ContextProvider.LogOut();
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                context.Response.Redirect(returnUrl ?? "/");
            }).RequireAuthorization();


            app.MapPost("/api/auth/token", async (
            LoginDataUser request,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, // opcional
            ITokenService tokenService) =>
            {
                // Usa request.Email, no request.Username
                var user = await userManager.FindByEmailAsync(request.Email);
                if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
                {
                    return Results.Unauthorized();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // o ClaimTypes.Sid
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("TenantId", (user.TenantId ?? 0).ToString()),
                    new Claim("DbKey", user.DbKey ?? "SqlServer"),
                    new Claim("AppState", user.AppState ?? "")
                };

                // Añadir roles
                var roles = await userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = tokenService.GenerateToken(claims);
                return Results.Ok(new { Token = token });
            }).AllowAnonymous();


        }
    }
}

