
using BlazorEbi9.Data;
using BlazorEbi9.Data.DataBase;
using BlazorEbi9.Data.Services;
using BlazorEbi9.Model.IServices;
using BlazorEbi9.Model.TenantService;
using BlazorEbi9.RestfullCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);

// Tenant services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

string providerApp = Configuration.GetValue(typeof(string), "DataProvider").ToString();
if (providerApp == "SqlServer")
{
    string connectionString = Configuration.GetConnectionString("SqlDbContext");
    builder.Services.AddDbContext<SqlServerDbContext>(options => options.UseSqlServer(connectionString));
    builder.Services.AddTransient<IUnitOfWorkAsync, UnitOfWorkAsync<SqlServerDbContext>>();
    builder.Services.AddTransient<IUsuarioServiceAsync, UsuarioServiceAsync>();
}
else if (providerApp == "SqLite")
{
    // Registrar el interceptor como singleton. NO resolver servicios scoped en el ctor del interceptor.
    builder.Services.AddSingleton<TenantSaveChangesInterceptor>();

    // Configure la fábrica: al crear el DbContext EF Core pedirá el interceptor singleton.
    builder.Services.AddDbContextFactory<SqLiteDbContext>((provider, options) =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("SqliteDbContext"));
 //       options.AddInterceptors(provider.GetRequiredService<TenantSaveChangesInterceptor>());
    });

    // Registrar UnitOfWork y servicios asociados (usar el DbContext correcto)
    builder.Services.AddTransient<IUnitOfWorkAsync, UnitOfWorkAsync<SqLiteDbContext>>();
    builder.Services.AddTransient<IUsuarioServiceAsync, UsuarioServiceAsync>();
}
else if (providerApp == "Restful")
{
    var urlApi = Configuration.GetConnectionString("UrlApi");
    builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceAsyncR(new HttpClient(), urlApi));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();