using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasoParametrosBlazor.Client.Pages;
using PasoParametrosBlazor.Components;

using Blazored.LocalStorage;
using PasoParametrosBlazor;
using PasoParametrosBlazor.Components;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<PasoParametrosBlazor.Client.AppState>();





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
app.MapRazorComponents<PasoParametrosBlazor.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(PasoParametrosBlazor.Client._Imports).Assembly);

app.Run();
