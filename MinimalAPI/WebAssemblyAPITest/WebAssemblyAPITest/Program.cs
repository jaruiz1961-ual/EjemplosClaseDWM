using WebAssemblyAPITest.Client.Services;
using WebAssemblyAPITest.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Configure the HTTP request pipeline.
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["BaseAddress"] ?? "https://localhost:7041")
});
builder.Services.AddScoped<CookieService>();


// Debes registrarlo también aquí para el Prerendering y el modo InteractiveServer
builder.Services.AddScoped<WebAssemblyAPITest.Client.Services.CookieService>();
builder.Services.AddScoped<WebAssemblyAPITest.Client.Services.ClientCodeService>();

var app = builder.Build();



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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebAssemblyAPITest.Client._Imports).Assembly);



app.Run();
