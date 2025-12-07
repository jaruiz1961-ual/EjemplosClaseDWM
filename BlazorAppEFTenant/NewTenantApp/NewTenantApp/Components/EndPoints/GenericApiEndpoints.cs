using DataBase.Genericos;
using DataBase.Modelo;
using DataBase.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Linq.Expressions;

namespace BlazorAppEFTenant.Components.EndPoints
{

    public static class GenericApiEndPoints
    {
        public static void GenericApis<T> (this WebApplication app) where T: class,ITenantEntity,IEntity, IUpdatableFrom<T>
        {   

            // GET: listar todos
            app.MapGet("/api/{contexto}/{nombreEntidad}", async (
                HttpContext httpContext,
                string contexto,
                string nombreEntidad,
                int tenantId,           
                IUnitOfWorkFactory uowFactory,
                 IContextProvider cp
                ) =>
                {
                if (!httpContext.User?.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }
                    cp._AppState.DbKey = contexto;
                    cp._AppState.TenantId = tenantId;
                    cp._AppState.ConnectionMode = "Ef";

                    var service = new GenericDataService<T>(cp, uowFactory);
                    try
                    {
                        var usuarios = await service.GetAllAsync();
                        return Results.Ok(usuarios);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return Results.Unauthorized();
                    }
                }); 

            // GET: listar filtro 
            app.MapGet("/api/{contexto}/{nombreEntidad}/filtrar", async (
                HttpContext httpContext,
                string contexto,
                string nombreEntidad,
                string filtro,
                int tenantId,
                IUnitOfWorkFactory uowFactory,
                IContextProvider cp  // ← DI


                ) =>
            {
                if (!httpContext.User?.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }
                cp._AppState.DbKey = contexto;
                cp._AppState.TenantId = tenantId;
                cp._AppState.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                var usuarios = await service.GetFilterAsync(filtro);
                return Results.Ok(usuarios);
            });

            // GET: obtener por id
            app.MapGet("/api/{contexto}/{nombreEntidad}/{id:int}", async (
                HttpContext httpContext,
                string contexto,
                string nombreEntidad,
                int id,
                int tenantId,           
                IUnitOfWorkFactory uowFactory,
                 IContextProvider cp
                ) =>
            {
                if (!httpContext.User?.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }
                cp._AppState.DbKey = contexto;
                cp._AppState.TenantId = tenantId;
                cp._AppState.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                try
                {
                    var usuario = await service.GetByIdAsync(id);
                    return usuario is not null ? Results.Ok(usuario) : Results.NotFound();

                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
              }).RequireAuthorization(); 

            // POST: crear usuario
            app.MapPost("/api/{contexto}/{nombreEntidad}", async (
                HttpContext httpContext,
                string contexto,
                string nombreEntidad,
                T usuario,
                int tenantId,
                IUnitOfWorkFactory uowFactory,
                 IContextProvider cp
                ) =>
            {
                if (!httpContext.User?.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }
                cp._AppState.DbKey = contexto;
                cp._AppState.TenantId = tenantId;
                cp._AppState.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                try
                {
                    await service.AddAsync(usuario);
                    return Results.Created($"/api/{contexto}/{nombreEntidad}/{usuario.Id}", usuario);

                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
        }); 

            // PUT: actualizar usuario
            app.MapPut("/api/{contexto}/{nombreEntidad}/{id:int}", async (
                HttpContext httpContext,
                string contexto,
                string nombreEntidad,
                int id,
                T usuario,
                int tenantId,
                IUnitOfWorkFactory uowFactory,
                 IContextProvider cp
                ) =>
            {
                if (!httpContext.User?.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }
                cp._AppState.DbKey = contexto;
                cp._AppState.TenantId = tenantId;
                cp._AppState.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                try
                {
                    var actual = await service.GetByIdAsync(id);
                    if (actual is null)
                        return Results.NotFound();
                    actual.UpdateFrom(usuario);
                    // Copia campos actualizables -.. no puedo hacer esto 
                    // porque T es desconocido necesito una interfaz que lo permita

                    await service.UpdateAsync(actual);
                    return Results.Ok(actual);

                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }

            }); 

            // DELETE: eliminar usuario
            app.MapDelete("/api/{contexto}/{nombreEntidad}/{id:int}", async (
                HttpContext httpContext,
                string contexto,
                string nombreEntidad,
                int id,
                int tenantId,
                IUnitOfWorkFactory uowFactory,
                 IContextProvider cp
                ) =>
            {
                if (!httpContext.User?.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }
                cp._AppState.DbKey = contexto;
                cp._AppState.TenantId = tenantId;
                cp._AppState.ConnectionMode = "Ef"; // Ajusta según tu lógica
                var service = new GenericDataService<T>(cp, uowFactory);
                var usuario = await service.GetByIdAsync(id);
                if (usuario is null)
                    return Results.NotFound();
                await service.DeleteAsync(id);
                try
                {
                    return Results.Ok(usuario);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
        }); 
        }
    }

}


