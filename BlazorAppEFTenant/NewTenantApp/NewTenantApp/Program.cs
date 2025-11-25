using BlazorAppEFTenant.Components;
using BlazorAppEFTenant.Components.EndPoints;
using DataBase.Contextos;
using DataBase.Genericos;
using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Razor y componentes Blazor (igual que en tu código original)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

IConfiguration configuration = builder.Configuration;

// HttpClient para APIs externas (AJUSTA la clave según tu appsettings)
Uri DirBase = new Uri(configuration.GetConnectionString("UrlApi") ?? "https://localhost:7013/");
builder.Services.AddHttpClient("ApiRest", (sp, client) =>
{
    client.BaseAddress = DirBase;
});



// Tenant Provider y Context Provider
builder.Services.AddScoped<ITenantProvider,TenantProvider>();

builder.Services.AddScoped<IContextKeyProvider,ContextKeyProvider>();

// Interceptor para multitenant (opcional)
builder.Services.AddScoped<TenantSaveChangesInterceptor>();

// Factorías de DbContexts para todos los backends locales:
builder.Services.AddDbContextFactory<SqlServerDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("SqlServerDbContext");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseSqlServer(conn);
    options.AddInterceptors(interceptor);
},ServiceLifetime.Transient);
builder.Services.AddDbContextFactory<SqLiteDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("SqLiteDbContext");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseSqlite(conn);
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Transient);
builder.Services.AddDbContextFactory<InMemoryDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("InMemoryDbContext");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseInMemoryDatabase(conn);
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Transient);

// NO REGISTRES IGenericRepository<,> como open-generic
// SÓLO REGISTRA LAS FACTORÍAS NECESARIAS:
builder.Services.AddScoped<IGenericRepositoryFactory, GenericRepositoryFactory>();
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

// OJO: Si tu UnitOfWork generic depende solo de DI-resolvable (DbContext y factory)
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

// Inicialización/Migración de BD al arrancar
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    void InitDb<TDb>(string factoryTypeKey, Action<TDb> migrator)
        where TDb : DbContext
    {
        var factory = scope.ServiceProvider.GetService<IDbContextFactory<TDb>>();
        var db = factory?.CreateDbContext();
        migrator(db);
    }

    InitDb<SqlServerDbContext>("SqlServer", db => db?.Database.Migrate());
    InitDb<SqLiteDbContext>("SqLite", db => db?.Database.Migrate());
    InitDb<InMemoryDbContext>("InMemory", db => db?.Database.EnsureCreated());
}
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorAppEFTenant.Client._Imports).Assembly);
app.MapUsuariosApis<Usuario>();

app.Run();
