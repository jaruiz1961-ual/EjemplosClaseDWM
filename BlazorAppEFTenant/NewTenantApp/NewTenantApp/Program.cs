using DataBase.Genericos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using BlazorAppEFTenant.Client.Pages;
using BlazorAppEFTenant.Components;
using System.Runtime.CompilerServices;
using DataBase.Contextos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
string provider = Configuration.GetValue(typeof(string), "DataProvider").ToString();



builder.Services.AddScoped<ITenantProvider>(sp =>
{
    // aquí decides cómo construirlo: qué ctor, qué valores por defecto, etc.
    var tenant = new TenantProvider();
    tenant.SetTenant(1); // Establece el TenantId por defecto o según la lógica que necesites
    return tenant;
});

builder.Services.AddScoped<IContextKeyProvider>(sp =>
{
    // aquí decides cómo construirlo: qué ctor, qué valores por defecto, etc.
    var context = new ContextKeyProvider();
    context.SetContextKey("InMemory"); // Establece el contextokey por defecto o según la lógica que necesites
    return context;
});

builder.Services.AddScoped<TenantSaveChangesInterceptor>();

//if (provider == "SqlServer")

//if (provider == "SqLite")
{
    // Configuración de cadena de conexión
    builder.Services.AddDbContextFactory<SqLiteDbContext>((sp, options) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("SqLiteDbContext");

        // Registrar el interceptor para la asignación automática de TenantId
        var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
        options.UseSqlite(connectionString);
        options.AddInterceptors(interceptor);

    },
    ServiceLifetime.Transient);
}

{
    // Configuración de cadena de conexión
    builder.Services.AddDbContextFactory<SqlServerDbContext>((sp, options) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("SqlServerDbContext");

        // Registrar el interceptor para la asignación automática de TenantId
        var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
        options.UseSqlServer(connectionString);
        options.AddInterceptors(interceptor);

    },
    ServiceLifetime.Transient);
}


{
    // Configuración de cadena de conexión
    builder.Services.AddDbContextFactory<InMemoryDbContext>((sp, options) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("InMemoryDbContext");

        // Registrar el interceptor para la asignación automática de TenantId
        var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
        options.UseInMemoryDatabase(connectionString);
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SqlServerDbContext>();
    db.Database.Migrate();          // Aplica todas las migraciones pendientes, crea la BD si no existe
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SqLiteDbContext>();
    db.Database.Migrate();          // Aplica todas las migraciones pendientes, crea la BD si no existe
}


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InMemoryDbContext>();
    db.Database.EnsureCreated();    // Para que funcione HasData
}


app.Run();
