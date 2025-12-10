using BlazorSeguridad2026.Data;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using Shares.Genericos;
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

        public static void LoginApis<T>(this WebApplication app) where T : class, ITenantEntity, IEntity, IUpdatableFrom<T>
        {
            app.MapGet("/Logout", async (HttpContext context, string? returnUrl, IContextProvider ContextProvider) =>
            {
                ContextProvider.LogOut();
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                context.Response.Redirect(returnUrl ?? "/");
            }).RequireAuthorization();


            app.MapPost("/api/auth/token", async (
            LoginData request,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, // opcional
            ITokenService tokenService) =>
            {
                // Usa request.Email, no request.Username
                var user = await userManager.FindByEmailAsync(request.email);
                if (user == null || !await userManager.CheckPasswordAsync(user, request.password))
                {
                    return Results.Unauthorized();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // o ClaimTypes.Sid
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("TenantId", (user.TenantId??0).ToString()),
                    new Claim("DbKey",user.DbKey??"SqlServer"),
                    new Claim("AppState",user.AppState??"")
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


            app.MapGet("/api/auth/login", async (
               HttpContext httpContext,
               string username,
               string password,
               IUnitOfWorkFactory uowFactory,
               IContextProvider cp, // ← DI
               ITokenService tokenService
               ) =>
           {
               ServicioSeguridad sc = new ServicioSeguridad(cp, uowFactory, tokenService);
               string filtro = $"UserName == \"{username}\" && Password == \"{password}\"";
               var listaUsuarios = await sc.GetFilterAsync(filtro);
               var user = listaUsuarios.FirstOrDefault();

               if (user == null)
                   return Results.Unauthorized();

               var categoria = user.Roles;

               // 2. Construir los claims
               var claims = new[]
               {
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email??""),
                    new Claim(ClaimTypes.Role, user.Roles ?? ""),
                    new Claim("TenantId", user.TenantId.ToString() ?? "0"),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Categoria", categoria??"")

               };

               var tokenString = tokenService.GenerateToken(claims);

               return Results.Ok(new { token = tokenString });
           })
           .AllowAnonymous();
        }
    }

}


