using DataBase.Genericos;
using DataBase.Modelo;
using DataBase.Servicios;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppEFTenant.Components.EndPoints
{

    public static class ApiEndpoints
    {
        public static void MapUsuariosApis<T> (this WebApplication app) where T: class,ITenantEntity,IEntity
        {
            // GET: listar todos
            app.MapGet("/api/{contexto}/usuarios", async (
                string contexto,
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
            });

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


