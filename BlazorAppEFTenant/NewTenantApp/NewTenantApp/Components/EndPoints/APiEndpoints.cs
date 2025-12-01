using DataBase.Genericos;
using DataBase.Modelo;
using DataBase.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimalapp;
using System.Linq.Expressions;

namespace BlazorAppEFTenant.Components.EndPoints
{

    public static class ApiEndpoints
    {
        public static void MapApisBase<T> (this WebApplication app) where T: class,ITenantEntity,IEntity
        {
            app.MapGet("secured-route", () => "Hello, you are authorized to see this!").RequireAuthorization();

            app.MapPost("/login", ([FromBody] string login, [FromServices] TokenService tokenService, HttpContext httpContext) =>
            {
                if (login != "admin") return Results.Unauthorized();
                var token = tokenService.GenerateToken(login);
                return Results.Ok(new { Token = token });
            });

            // GET: listar todos
            app.MapGet("/api/{contexto}/{nombreEntidad}", async (
                string contexto,
                string nombreEntidad,
                int tenantId,           
                IUnitOfWorkFactory uowFactory


                ) =>
            {
                var cp = new ContextProvider();
                cp.DbKey = contexto;
                cp.TenantId = tenantId;
                cp.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                var usuarios = await service.GetAllAsync();
                return Results.Ok(usuarios);
            }).RequireAuthorization();

            // GET: listar filtro 
            app.MapGet("/api/{contexto}/{nombreEntidad}", async (
                string contexto,
                string nombreEntidad,
                Expression <Func<T, bool>> predicate,
                int tenantId,
                IUnitOfWorkFactory uowFactory


                ) =>
            {
                var cp = new ContextProvider();
                cp.DbKey = contexto;
                cp.TenantId = tenantId;
                cp.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                var usuarios = await service.GetFilterAsync(predicate);
                return Results.Ok(usuarios);
            }).RequireAuthorization();

            // GET: obtener por id
            app.MapGet("/api/{contexto}/usuarios/{id:int}", async (
                string contexto,
                int id,
                int tenantId,           
                IUnitOfWorkFactory uowFactory
                ) =>
            {
                var cp = new ContextProvider();
                cp.DbKey = contexto;
                cp.TenantId = tenantId;
                cp.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                var usuario = await service.GetByIdAsync(id);
                return usuario is not null ? Results.Ok(usuario) : Results.NotFound();
            });

            // POST: crear usuario
            app.MapPost("/api/{contexto}/usuarios", async (
                string contexto,
                T usuario,
                int tenantId,
                IUnitOfWorkFactory uowFactory
                ) =>
            {
                var cp = new ContextProvider();
                cp.DbKey = contexto;
                cp.TenantId = tenantId;
                cp.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                await service.AddAsync(usuario);
                return Results.Created($"/api/{contexto}/usuarios/{usuario.Id}", usuario);
            });

            // PUT: actualizar usuario
            app.MapPut("/api/{contexto}/usuarios/{id:int}", async (
                string contexto,
                int id,
                T usuario,
                int tenantId,
                IUnitOfWorkFactory uowFactory
                ) =>
            {
                var cp = new ContextProvider();
                cp.DbKey = contexto;
                cp.TenantId = tenantId;
                cp.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                var actual = await service.GetByIdAsync(id);
                if (actual is null)
                    return Results.NotFound();
                // Copia campos actualizables
                actual = usuario;
                //actual.UserName = usuario.UserName;
                //actual.Codigo = usuario.Codigo;
                //actual.Contexto = usuario.Contexto;
                //actual.Password = usuario.Password;
                //actual.TenantId = usuario.TenantId;
                await service.UpdateAsync(actual);
                return Results.Ok(actual);
            });

            // DELETE: eliminar usuario
            app.MapDelete("/api/{contexto}/usuarios/{id:int}", async (
                string contexto,
                int id,
                int tenantId,
                IUnitOfWorkFactory uowFactory
                ) =>
            {
                var cp = new ContextProvider();
                cp.DbKey = contexto;
                cp.TenantId = tenantId;
                cp.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                var usuario = await service.GetByIdAsync(id);
                if (usuario is null)
                    return Results.NotFound();
                await service.DeleteAsync(id);
                return Results.Ok(usuario);
            });
        }
    }

}


