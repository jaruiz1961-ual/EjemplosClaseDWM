using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAssemblyAPITest;
using WebAssemblyAPITest.Client;
using WebAssemblyAPITest.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<CookieService>();
builder.Services.AddTransient<CookieBearerTokenHandler>();

builder.Services.AddHttpClient("ApiCliente", client =>
{
    client.BaseAddress = new Uri("https://localhost:7242"); //  API
})
    .AddHttpMessageHandler<CookieBearerTokenHandler>();



await builder.Build().RunAsync();


