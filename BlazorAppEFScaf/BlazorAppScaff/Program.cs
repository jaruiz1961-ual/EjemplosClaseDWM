using BlazorAppScaff.Components;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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

//PM> NuGet\Install-Package Microsoft.EntityFrameworkCore.Tools -Version 9.0.0
//PM> Nuget\Install-Package  Microsoft.EntityFrameworkCore.Design
//PM> dotnet tool install --global dotnet-ef
//PM> Install-Package Microsoft.EntityFrameworkCore.SqlServer
//
//Scaffold-DbContext "Data Source=(localdb)\MssqlLocaldb;Initial Catalog=Test;AttachDbFileName=C:\\Temp\\TestDb.mdf ;Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
//add-migration Inicial
//update-database
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
