using BlazorEbi7.Model.IServices;
using BlazorEbi7.RestfullCore.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR(new HttpClient(), "https://localhost:5001/api/"));


await builder.Build().RunAsync();
