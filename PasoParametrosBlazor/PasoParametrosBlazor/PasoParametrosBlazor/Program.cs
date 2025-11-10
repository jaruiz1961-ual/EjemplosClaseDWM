using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasoParametrosBlazor.Client.Pages;
using PasoParametrosBlazor.Components;
<<<<<<< HEAD
using Blazored.LocalStorage;
using LibreriaCompartida;
using PasoParametrosBlazor;
using PasoParametrosBlazor.Components;

=======
using RazorClassLibrary;
using StackExchange.Redis;
using System.Security.Claims;
>>>>>>> actualizacion desde universidad pasoParametrosBlazor

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

<<<<<<< HEAD
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<PasoParametrosBlazor.Client.AppState>();



=======
builder.Services.AddSingleton<AppState>();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddAuthorization();
var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IDatabase>(redis.GetDatabase());
>>>>>>> actualizacion desde universidad pasoParametrosBlazor

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
// GET endpoint para recuperar el valor
app.MapGet("/user/value", [Authorize] (ClaimsPrincipal user, IDatabase redis) =>
{
    // Reemplaza con la clave real de usuario según tu sistema
    var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonimo";
    var val = redis.StringGet($"user:{userId}:value");
    return Results.Ok(val.HasValue ? val.ToString() : "");
});

// POST endpoint para guardar el valor
app.MapPost("/user/value", [Authorize] (ClaimsPrincipal user, IDatabase redis, [FromBody] string nuevoValor) =>
{
    var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonimo";
    redis.StringSet($"user:{userId}:value", nuevoValor);
    return Results.NoContent();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<PasoParametrosBlazor.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(PasoParametrosBlazor.Client._Imports).Assembly);

app.Run();
