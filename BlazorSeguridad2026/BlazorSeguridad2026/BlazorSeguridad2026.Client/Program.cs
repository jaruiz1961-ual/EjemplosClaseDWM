
using Blazored.LocalStorage;
using BlazorSeguridad2026.Base.Contextos;
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Base.Seguridad;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Net.Http.Json;
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

builder.Services.AddScoped<IMultiDbContextFactory>(sp =>
{
    //var map = new Dictionary<string, Func<DbContext>>
    //{
    //    ["application"] = () =>
    //        sp.GetRequiredService<IDbContextFactory<ApplicationBaseDbContext>>()
    //          .CreateDbContext(),

    //    ["sqlserver"] = () =>
    //        sp.GetRequiredService<IDbContextFactory<SqlServerDbContext>>()
    //          .CreateDbContext(),

    //    ["sqlite"] = () =>
    //        sp.GetRequiredService<IDbContextFactory<SqLiteDbContext>>()
    //          .CreateDbContext(),

    //    ["inmemory"] = () =>
    //        sp.GetRequiredService<IDbContextFactory<InMemoryBaseDbContext>>()
    //          .CreateDbContext()
    //};

    return new MultiDbContextFactory(null);
});


builder.Services.AddBlazoredLocalStorage();

// Configuración
IConfiguration configuration = builder.Configuration;
var UrlApi = configuration["ConnectionStrings:UrlApi"] ?? "https://localhost:7013/";
var ApiName = configuration["ConnectionStrings:ApiName"] ?? "ApiRest";
var ConnectionMode = configuration["ConnectionStrings:ConnectionMode"] ?? "Ef";
var DataProvider = configuration["DataProvider"] ?? "SqlServer";
var TenantId = configuration["TenantId"] ?? "0";

var serverMode = false;

builder.Services.AddScoped<ContextProvider>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();

    var initialServerState = new State
    {
        TenantId = int.Parse(TenantId),
        DbKey = DataProvider,
        ConnectionMode = ConnectionMode,
        ApiName = ApiName,
        DirBase = new Uri(UrlApi)
    };

    var initialClientState = new State
    {
        TenantId = int.Parse(TenantId),
        DbKey = DataProvider,
        ConnectionMode = "Api",
        ApiName = ApiName,
        DirBase = new Uri(UrlApi)
    };


    var cp = new ContextProvider(localStorage, true);
    cp.States[0] = initialClientState;
    cp.States[1] = initialServerState;

    return cp;
});
builder.Services.AddScoped<IContextProvider>(sp => sp.GetRequiredService<ContextProvider>());


builder.Services.AddScoped(typeof(IGenericRepositoryFactoryAsync<>), typeof(GenericRepositoryFactory<>));

// Factoría de UoW
builder.Services.AddScoped(typeof(IUnitOfWorkAsync), typeof(UnitOfWorkAsync));
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

var host = builder.Build();

// HttpClient del host para llamar a la minimal API de cultura
var http = host.Services.GetRequiredService<HttpClient>();

// Si tu minimal API devuelve DTO { culture, uiCulture }
var cultureResponse = await http.GetFromJsonAsync<CultureInfoDto>(
    $"{apiUrl}Culture/Get");

var cultureName = cultureResponse?.Culture ?? "es-ES";

var culture = new CultureInfo(cultureName);
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// Opcional: guardar en localStorage/appstate para reutilizar
var localStorageSrv = host.Services.GetRequiredService<ILocalStorageService>();
var appState = await localStorageSrv.GetItemAsync<State>("appstate") ?? new State();
appState.Culture = cultureName;
await localStorageSrv.SetItemAsync("appstate", appState);

// Ejecutar la app
await host.RunAsync();

// DTO para la respuesta de la minimal API
public class CultureInfoDto
{
    public string Culture { get; set; } = default!;
    public string UICulture { get; set; } = default!;
}