using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using BlazorEbi9.Data;
using BlazorEbi9.Data.DataBase;
using BlazorEbi9.Model.Entidades;
using BlazorEbi9.Model.IServices;

namespace BlazorEbi9.API.Controllers
{

    public static class UsersController
    {

        public static void Map(WebApplication app)
        {
            app.MapGet("/api/users", async (IUsuarioServiceAsync userService) =>
            {
                var users = await userService.FindAllAsync();
                return users;
            });

            app.MapPost("/api/users", async (IUsuarioServiceAsync userService, UsuarioSet users) =>
            {
                // Validación manual básica (puedes agregar más condiciones según tus DataAnnotations)
                if (users == null)
                {
                    return Results.BadRequest("User cannot be null.");
                }

                // Si quieres validar propiedades:
                if (string.IsNullOrWhiteSpace(users.UserName))
                {
                    return Results.BadRequest("User name is required.");
                }
                users.Id = 0; 
                // Guarda el usuario
                var result = await userService.SaveUserAsync(users);

                // Si el servicio devuelve el usuario correctamente, retorna OK
                if (result != null)
                {
                    return Results.Ok(result);
                }
                else
                {
                    return Results.BadRequest("Error in adding user!");
                }
            });


            app.MapGet("/api/users/{id:int}", async (IUsuarioServiceAsync userService, int id) =>
            {
                if (id > 0)
                {
                    var model = await userService.FindIdAsync(id);
                    return model is not null ? Results.Ok(model) : Results.NotFound();
                }
                return Results.BadRequest("Invalid user id!");
            });

            app.MapPut("/api/users/{id:int}", async (IUsuarioServiceAsync userService, int id, UsuarioSet users) =>
            {
                // Validación básica, agrega validación personalizada si lo necesitas.
                if (users == null || id != users.Id)
                    return Results.BadRequest("Error in updating user!");

                var result = await userService.SaveUserAsync(users);
                return result != null
                    ? Results.Ok(true)
                    : Results.BadRequest("Error in updating user!");
            });

            app.MapDelete("/api/users/{id:int}", async (IUsuarioServiceAsync userService, int id) =>
            {
                if (id > 0)
                {
                    var model = await userService.FindIdAsync(id);
                    if (model != null)
                    {
                        var result = await userService.DeleteIdAsync(id);
                        return result ? Results.Ok("User deleted!") : Results.BadRequest("Error in deleting user!");
                    }
                    else
                    {
                        return Results.BadRequest("User does not exist!");
                    }
                }
                return Results.BadRequest("Invalid user id!");
            });



        }
    }
}