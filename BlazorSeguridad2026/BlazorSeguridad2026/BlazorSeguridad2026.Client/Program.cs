
using Blazored.LocalStorage;
using Cliente;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.NetworkInformation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

var apiUrl = builder.Configuration["ConnectionStrings:UrlApi"];


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});


builder.Services.AddHttpClient("ApiRest", client =>
{
    client.BaseAddress = new Uri(apiUrl);
});


builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IContextProvider, ContextProvider>();
await builder.Build().RunAsync();
