using Blazored.LocalStorage;
using LibreriaCompartida;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using PasoParametrosBlazor.Client;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton<AppState>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AppState>();


await builder.Build().RunAsync();
