using BlazorEbi9.Data;
using BlazorEbi9.Data.DataBase;
using BlazorEbi9.Data.Services;
using BlazorEbi9.Model.IServices;
using BlazorEbi9.RestfullCore.Services;
using Microsoft.EntityFrameworkCore;

IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);


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
