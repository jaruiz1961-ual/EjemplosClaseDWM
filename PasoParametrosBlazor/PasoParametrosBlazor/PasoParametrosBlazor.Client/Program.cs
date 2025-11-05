using Blazored.LocalStorage;
using LibreriaCompartida;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazoredLocalStorage();



await builder.Build().RunAsync();
