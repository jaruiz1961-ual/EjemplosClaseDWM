using BlazorAppEFTenant.Components;
using DataBase.Genericos;
using Microsoft.EntityFrameworkCore; // Agrega esta directiva using al inicio del archivo

var builder = WebApplication.CreateBuilder(args);

IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
// Registrar el TenantProvider (puede ser Scoped, depende de cómo lo resuelvas en cada request o session)

builder.Services.AddTransient<ITenantProvider>(sp =>
{
    var provider = new TenantProvider();
    provider.SetTenant(1);
    return provider;
});


// Registrar el interceptor como scoped (depende de ITenantProvider)
builder.Services.AddScoped<TenantSaveChangesInterceptor>();

string provider = Configuration.GetValue(typeof(string), "DataProvider").ToString();
builder.Services.AddSingleton(provider);

builder.Services.AddDbContext<SqlServerContext>(ServiceLifetime.Transient);


if (provider == "SqlServer")
{
    // Configuración de cadena de conexión
    builder.Services.AddDbContext<SqlServerContext>((sp, options) =>
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

// Registrar repositorios y unidad de trabajo
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

// Registrar servicios y componentes de Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();





// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();
