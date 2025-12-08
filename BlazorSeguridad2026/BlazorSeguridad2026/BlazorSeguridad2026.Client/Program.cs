using Blazored.LocalStorage;
using DataBase;
using DataBase.Genericos;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.NetworkInformation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();



builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<DataBase.Genericos.ContextProvider>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();


    var cp = new ContextProvider(localStorage)
    {
        _AppState = new AppState
        {
            TenantId = 0,
            DbKey = "SqlServer",
            ConnectionMode = "Ef",
            ApiName = "ApiRest",
            DirBase = new Uri("https://localhost:7013/"),
            Token = null
        }
    };

    return cp;
});
builder.Services.AddScoped<IContextProvider>(sp => sp.GetRequiredService<ContextProvider>());

await builder.Build().RunAsync();
