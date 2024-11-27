using BlazorEbi9.WebAssembly;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorEbi9.RestfullCore.Services;
using BlazorEbi9.Data.Services;
using BlazorEbi9.Model.IServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR(new HttpClient(), "https://localhost:5001/api/"));


//builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();
//var HttpsClientHandS = builder.Services.BuildServiceProvider().GetService<IHttpsClientHandlerService>() as IHttpsClientHandlerService;
//builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR (HttpsClientHandS, "https://localhost:5001/api/"));

await builder.Build().RunAsync();
