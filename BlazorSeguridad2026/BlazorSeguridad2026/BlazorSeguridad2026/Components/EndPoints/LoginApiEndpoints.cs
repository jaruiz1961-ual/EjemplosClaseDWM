using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
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
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

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
            RoleManager<ApplicationRole> roleManager, // opcional
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
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // o ClaimTypes.Sid
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

         //NO PUEDO USR JAVASCRIPT EN MINIMAL APIS .... 
         //  app.MapPost("/Tenant/Change", async (
         //[FromForm] int tenantId,
         //HttpContext http,
         //IContextProvider ContextProvider) =>
         //   {
         //       //await ContextProvider.ReadAllContext();
         //       //ContextProvider._AppState.TenantId = tenantId;
         //       //await ContextProvider.SaveAllContextAsync(ContextProvider);
         //       var referer = http.Request.Headers.Referer.ToString();
         //       var url = string.IsNullOrEmpty(referer) ? "/" : referer;

         //       return Results.Redirect(url);
         //   }).RequireAuthorization();

        }
    }
}

