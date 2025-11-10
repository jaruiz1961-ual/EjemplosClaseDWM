using Blazored.LocalStorage;
using LibreriaCompartida;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
<<<<<<< HEAD
using PasoParametrosBlazor.Client;
=======
using RazorClassLibrary;
>>>>>>> actualizacion desde universidad pasoParametrosBlazor

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton<AppState>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AppState>();


await builder.Build().RunAsync();
