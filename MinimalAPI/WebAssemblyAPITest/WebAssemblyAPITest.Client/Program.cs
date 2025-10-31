using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAssemblyAPITest;
using WebAssemblyAPITest.Client;
using WebAssemblyAPITest.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped(sp => new HttpClient
{
    // ¡DEBE APUNTAR AL PUERTO DE TU MINIMAL API (SERVIDOR)!
    BaseAddress = new Uri("https://localhost:7242")
});

builder.Services.AddSingleton<CookieService>();

await builder.Build().RunAsync();


