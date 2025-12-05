using DataBase;
using DataBase.Genericos;
using DataBase.Modelo;
using DataBase.Servicios;
using Microsoft.AspNetCore.Http;
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

namespace BlazorAppEFTenant.Components.EndPoints
{

    public static class LoginApiEndpoints
    {
        public static void LoginApis<T>(this WebApplication app) where T : class, ITenantEntity, IEntity, IUpdatableFrom<T>
        {
            app.MapGet("/login", async (
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

               if (user==null)
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
   
               var tokenString = sc.GenerateToken(claims);

               return Results.Ok(new { token = tokenString });
           })
           .AllowAnonymous();
        }
    }

}


