
using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shares.Genericos;
using System.Net.NetworkInformation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var apiUrl = builder.Configuration["ConnectionStrings:UrlApi"];




builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();




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

builder.Services.AddScoped(typeof(IGenericRepositoryFactory<>), typeof(GenericRepositoryFactory<>));

// Factoría de UoW
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

await builder.Build().RunAsync();
