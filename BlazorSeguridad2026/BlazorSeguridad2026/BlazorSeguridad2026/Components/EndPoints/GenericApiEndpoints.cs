
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shares.Genericos;
using Shares.Modelo;
using Shares.Seguridad;
using Shares.Servicios;
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
                 IContextProvider cp,
                 bool reload = false
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
                        var usuarios = await service.ObtenerTodosAsync(reload);
                        return Results.Ok(usuarios);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return Results.Unauthorized();
                    }
                }).RequireAuthorization(); 

            // GET: listar filtro 
            app.MapGet("/api/{contexto}/{nombreEntidad}/filtrar", async (
                HttpContext httpContext,
                string contexto,
                string nombreEntidad,
                string filtro,
                int tenantId,
                IUnitOfWorkFactory uowFactory,
                IContextProvider cp, // ← DI,
                bool reload = false


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
                var usuarios = await service.ObtenerFiltradosCadenaAsync(filtro,reload);
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
                IContextProvider cp, // ← DI,
                bool reload = false
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
                    var usuario = await service.ObtenerPorIdAsync(id, reload);
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
                 IContextProvider cp,
                  bool reload = false
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
                    await service.AñadirAsync(usuario,reload);
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
                 IContextProvider cp,
                  bool reload = false
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
                    var actual = await service.ObtenerPorIdAsync(id,reload);
                    if (actual is null)
                        return Results.NotFound();
                    actual.UpdateFrom(usuario);
                    // Copia campos actualizables -.. no puedo hacer esto 
                    // porque T es desconocido necesito una interfaz que lo permita

                    await service.ActualizarAsync(actual, reload);
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
                 IContextProvider cp,
                  bool reload = false
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
                var usuario = await service.ObtenerPorIdAsync(id,reload);
                if (usuario is null)
                    return Results.NotFound();
                await service.EliminarAsync(id,reload);
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


