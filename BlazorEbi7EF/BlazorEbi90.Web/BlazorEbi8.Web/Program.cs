using BlazorEbi7.Data.DataBase;
using BlazorEbi7.Data.Services;
using BlazorEbi7.Model.IServices;
using BlazorEbi7.RestfullCore.Services;
using BlazorEbi8.Web.Client.Pages;
using BlazorEbi8.Web.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

string provider = Configuration.GetValue(typeof(string), "SqlProvider").ToString();
if (provider == "SqlServer")
{
    builder.Services.AddDbContext<SqlServerDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlDbContext")));
    builder.Services.AddTransient<IUnitOfWorkAsync, UnitOfWorkAsync<SqlServerDbContext>>();
}
else
{
    builder.Services.AddDbContext<SqLiteDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqLiteDbContext")));
    builder.Services.AddTransient<IUnitOfWorkAsync, UnitOfWorkAsync<SqLiteDbContext>>();
}

builder.Services.AddTransient<IUsuarioServiceAsync, UsuarioServiceAsync>();

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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
