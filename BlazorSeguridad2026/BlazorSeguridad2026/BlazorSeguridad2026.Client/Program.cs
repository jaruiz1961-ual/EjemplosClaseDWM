
using Blazored.LocalStorage;
using BlazorSeguridad2026.Base.Contextos;
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Base.Seguridad;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var apiUrl = builder.Configuration["ConnectionStrings:UrlApi"];


builder.Services.AddBootstrapBlazor(); 

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

//builder.Services.AddScoped<IMultiDbContextFactory>(sp =>
//{
//    var map = new Dictionary<string, Func<DbContext>>
//    {
//        ["application"] = () =>
//            sp.GetRequiredService<IDbContextFactory<ApplicationBaseDbContext>>()
//              .CreateDbContext(),

//        ["sqlserver"] = () =>
//            sp.GetRequiredService<IDbContextFactory<SqlServerDbContext>>()
//              .CreateDbContext(),

//        ["sqlite"] = () =>
//            sp.GetRequiredService<IDbContextFactory<SqLiteDbContext>>()
//              .CreateDbContext(),

//        ["inmemory"] = () =>
//            sp.GetRequiredService<IDbContextFactory<InMemoryBaseDbContext>>()
//              .CreateDbContext()
//    };

//    return new MultiDbContextFactory(map);
//});


builder.Services.AddBlazoredLocalStorage();

// Configuración
IConfiguration configuration = builder.Configuration;
var UrlApi = configuration["ConnectionStrings:UrlApi"] ?? "https://localhost:7013/";
var ApiName = configuration["ConnectionStrings:ApiName"] ?? "ApiRest";
var ConnectionMode = "Api";
var DataProvider = configuration["DataProvider"] ?? "SqlServer";
var TenantId = configuration["TenantId"] ?? "0";

builder.Services.AddScoped<IContextProvider>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();

    var initialState = new AppState
    {
        TenantId = int.Parse(TenantId),
        DbKey = DataProvider,
        ConnectionMode = ConnectionMode,
        ApiName = ApiName,
        DirBase = new Uri(UrlApi),
        Token = null
    };

    var cp = new ContextProvider(localStorage)
    {
        AppState = initialState
    };

    return cp;
});

// (Opcional) si quieres también el tipo concreto:
builder.Services.AddScoped<ContextProvider>(sp =>
    (ContextProvider)sp.GetRequiredService<IContextProvider>());



builder.Services.AddScoped(typeof(IGenericRepositoryFactoryAsync<>), typeof(GenericRepositoryFactory<>));

// Factoría de UoW
builder.Services.AddScoped(typeof(IUnitOfWorkAsync), typeof(UnitOfWorkAsync));
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

await builder.Build().RunAsync();
