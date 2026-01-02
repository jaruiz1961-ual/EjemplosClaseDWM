using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Data;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace BlazorAppEFTenant.Components.EndPoints
{
    public record SetCultureRequest(string Culture, string RedirectUri);
    public static class LoginApiEndpoints
    {
        
        public static void LoginApis(this WebApplication app)
        {
            app.MapGet("/Culture/Get", () =>
            {
                var culture = CultureInfo.CurrentCulture.Name;
                var uiCulture = CultureInfo.CurrentUICulture.Name;

                return Results.Ok(new
                {
                    Culture = culture,
                    UICulture = uiCulture
                });
            });
            app.MapGet("/Culture/Set", (string culture, string redirectUri, HttpContext httpContext
                , IContextProvider ContextProvider) =>
            {
   
                if (!string.IsNullOrEmpty(culture))
                {
                    httpContext.Response.Cookies.Append(
                        CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddYears(1)
                        });

                    // parece que no puedo utilizar localstorage en las minimal APIs !!!!!
                    //ContextProvider.ReadAllContext(true);
                    //ContextProvider.SetClaveValor(ClavesEstado.Culture, culture);
                    //ContextProvider.UpdateEstadoContext();
                }

                var unescapedUrl = Uri.UnescapeDataString(redirectUri ?? "/");
                return Results.LocalRedirect(unescapedUrl);
            });



            //// no puedo llamar en las apis a localresources ni nada pareceido

            //app.MapGet("/Account/Logout", async (HttpContext context, string? returnUrl, IContextProvider ContextProvider) =>
            //{
               
            //    await context.SignOutAsync(IdentityConstants.ApplicationScheme);
            //    context.Response.Redirect(returnUrl ?? "/Logout");
              
            //}).RequireAuthorization();


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

