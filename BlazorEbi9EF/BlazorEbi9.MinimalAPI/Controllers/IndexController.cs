using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorEbi9.Data;
using BlazorEbi9.Model;


namespace BlazorEbi9.MinimalAPI
{
    public static class IndexController
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/", async context =>
            {
                // Get all todo items
                await context.Response.WriteAsJsonAsync("Bienvenido a Minimal API");
            });

        }
    }
}