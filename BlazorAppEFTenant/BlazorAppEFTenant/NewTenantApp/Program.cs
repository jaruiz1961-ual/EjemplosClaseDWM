using DataBase.Genericos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using BlazorAppEFTenant.Client.Pages;
using BlazorAppEFTenant.Components;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
string provider = Configuration.GetValue(typeof(string), "DataProvider").ToString();

builder.Services.AddScoped<ITenantProvider, TenantProvider>();

builder.Services.AddScoped<ITenantProvider>(sp =>
{
    // aquí decides cómo construirlo: qué ctor, qué valores por defecto, etc.
    var tenant = new TenantProvider();
    tenant.SetTenant(1); // Establece el TenantId por defecto o según la lógica que necesites
    return tenant;
});

builder.Services.AddScoped<TenantSaveChangesInterceptor>();

if (provider == "SqlServer")
{
    // Configuración de cadena de conexión
    builder.Services.AddDbContextFactory<SqlDbContext>((sp, options) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("SqlDbContext");

        // Registrar el interceptor para la asignación automática de TenantId
        var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
        options.UseSqlServer(connectionString);
        options.AddInterceptors(interceptor);

    },
    ServiceLifetime.Transient);
}



builder.Services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();








var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorAppEFTenant.Client._Imports).Assembly);

app.Run();
