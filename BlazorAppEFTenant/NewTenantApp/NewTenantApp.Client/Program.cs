using DataBase.Contextos;
using DataBase.Genericos;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;

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
Uri DirBase = new Uri(apiUrl!);
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = DirBase }
);






builder.Services.AddScoped<IContextProvider>(sp =>
{
    var provider = new ContextProvider();
    provider.SetContext(1,"InMemory","ApiRest",new Uri(@"https://localhost:7031/"),"ApiServer"); // Asigna aquí el valor inicial por defecto
    return provider;
});



// NO REGISTRES IGenericRepository<,> como open-generic
// SÓLO REGISTRA LAS FACTORÍAS NECESARIAS:
builder.Services.AddScoped<IGenericRepositoryFactory, GenericRepositoryFactory>();
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

// OJO: Si tu UnitOfWork generic depende solo de DI-resolvable (DbContext y factory)
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));





await builder.Build().RunAsync();
