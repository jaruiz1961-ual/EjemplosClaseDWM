using Blazored.LocalStorage;
using DataBase.Contextos;
using DataBase.Genericos;
using DataBase.Servicios;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using DataBase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


// HttpClient para APIs externas (AJUSTA la clave según tu appsettings)
//builder.Services.AddHttpClient("ApiRest", (sp, client) =>
//{
//    var config = sp.GetRequiredService<IConfiguration>();
//    var UrlApi = config.GetConnectionString("UrlApi");
//    Console.WriteLine("UrlApi: " + config.GetConnectionString("UrlApi"));
//    client.BaseAddress = new Uri(UrlApi);
//});
var apiUrl = builder.Configuration["ConnectionStrings:UrlApi"];
builder.Services.AddTransient<CookieBearerTokenHandler>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});


builder.Services.AddHttpClient("ApiRest", client =>
{
    client.BaseAddress = new Uri(apiUrl);
}).AddHttpMessageHandler<CookieBearerTokenHandler>();


builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IContextProvider, ContextProvider>();



// NO REGISTRES IGenericRepository<,> como open-generic ??

builder.Services.AddScoped(typeof(IGenericRepositoryFactory<>), typeof(GenericRepositoryFactory<>));

// Factoría de UoW
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

// y, si ServicioUsuariosCliente es una especialización, también su propio registro concreto



// OJO: Si tu UnitOfWork generic depende solo de DI-resolvable (DbContext y factory)






await builder.Build().RunAsync();
