using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorEbi9.Data.DataBase;
using BlazorEbi9.Data.Services;
using Microsoft.EntityFrameworkCore;
using BlazorEbi9.Data;
using BlazorEbi9.Model.IServices;
using BlazorEbi9.RestfullCore.Services;

using BlazorEbi9.ServerBlazorise; // Asegúrate de añadir la referencia de proyecto para que 'App' esté disponible

IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Registrar soporte de Razor Components interactivo
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);

// Registrar antiforgery (CSRF)
builder.Services.AddAntiforgery(options =>
{
    // Nombre de cabecera usado por el cliente (opcional, ajustar si es necesario)
    options.HeaderName = "X-CSRF-TOKEN";
});

string provider = Configuration.GetValue(typeof(string), "DataProvider").ToString();
if (provider == "SqlServer")
{
    builder.Services.AddDbContext<SqlServerDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlDbContext")));
    builder.Services.AddTransient<IUnitOfWorkAsync, UnitOfWorkAsync<SqlServerDbContext>>();
    builder.Services.AddTransient<IUsuarioServiceAsync, UsuarioServiceAsync>();
}
else if (provider == "SqLite")
{
    builder.Services.AddDbContext<SqLiteDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqLiteDbContext")));
    builder.Services.AddTransient<IUnitOfWorkAsync, UnitOfWorkAsync<SqLiteDbContext>>();
    builder.Services.AddTransient<IUsuarioServiceAsync, UsuarioServiceAsync>();
}
else if (provider == "Restful")
{
    var urlApi = Configuration.GetConnectionString("UrlApi");
    builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR(new HttpClient(), urlApi));
}

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

// IMPORTANT: UseAntiforgery debe ir entre UseRouting() y el mapeo de endpoints.
// Si usas autenticación/autorización, llama antes a app.UseAuthentication()/app.UseAuthorization().
app.UseAntiforgery();

// Mapear endpoints para Razor Components interactivos — server render mode (si procede)
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
