using BlazorEbi9.Model.IServices;
using BlazorEbi9.RestfullCore.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceAsyncR(new HttpClient(), "https://localhost:5001/api/"));


await builder.Build().RunAsync();
