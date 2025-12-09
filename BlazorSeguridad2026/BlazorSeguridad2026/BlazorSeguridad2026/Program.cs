using Blazored.LocalStorage;
using BlazorSeguridad2026.Client.Pages;
using BlazorSeguridad2026.Components;
using BlazorSeguridad2026.Components.Account;
using BlazorSeguridad2026.Data;


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Shares.Genericos;
using Shares.SeguridadToken;
using System.Configuration;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//Acceso al archivo de configuracion appsetings.json
IConfiguration configuration = builder.Configuration;
var UrlApi = builder.Configuration["ConnectionStrings:UrlApi"] ?? "https://localhost:7013/";
var ApiName = builder.Configuration["ConnectionStrings:ApiName"] ?? "ApiRest";
var ConnectionMode = builder.Configuration["ConnectionStrings:ConnectionMode"] ?? "Ef";
var DataProvider = builder.Configuration["DataProvider"] ?? "SqlServer";
var TenantId = builder.Configuration["TenantId"] ?? "0";

//configuracion de la seguridad de Identidad de ASP.NET Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");




// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

// Configuración de la autenticación y autorización (con roles)

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
.AddRoles<IdentityRole>() // si se quiere roles
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Generar toquen para acceso a las MInimal APIS del servidor desde el cliente Blazor
builder.Services.AddSingleton<ITokenService, TokenService>();

// Configuración del Local Storage
builder.Services.AddBlazoredLocalStorage();

// Configuración del ContextProvider de la aplicación
builder.Services.AddScoped<ContextProvider>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var cp = new ContextProvider(localStorage)
    {
        _AppState = new AppState
        {
            TenantId = int.Parse(TenantId),
            DbKey = DataProvider,
            ConnectionMode = ConnectionMode,
            ApiName = ApiName,
            DirBase = new Uri(UrlApi),
            Token = null
        }
    };

    return cp;
});
builder.Services.AddScoped<IContextProvider>(sp => sp.GetRequiredService<ContextProvider>());

// Configuración del HttpClient para llamadas a la API
builder.Services.AddHttpClient(ApiName, (sp, client) =>
{
    client.BaseAddress = new Uri(UrlApi);
});




var app = builder.Build();


// Minimal API para autenticación en APIs y generación de token JWT

app.MapGet("/Logout", async (HttpContext context, string? returnUrl, IContextProvider ContextProvider) =>
{
    ContextProvider.LogOut();
    await context.SignOutAsync(IdentityConstants.ApplicationScheme);
    context.Response.Redirect(returnUrl ?? "/");
}).RequireAuthorization();

app.MapPost("/api/auth/token", async (
 LoginData request,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager, // opcional
    ITokenService tokenService) =>
{
// Usa request.Email, no request.Username
var user = await userManager.FindByEmailAsync(request.email);
if (user == null || !await userManager.CheckPasswordAsync(user, request.password))
{
    return Results.Unauthorized();
}

var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id), // o ClaimTypes.Sid
    new Claim(ClaimTypes.Name, user.UserName ?? ""),
    new Claim(ClaimTypes.Email, user.Email ?? ""),
    new Claim("TenantId", (user.TenantId??0).ToString()),
    new Claim("DbKey",user.DbKey??"SqlServer"),
    new Claim("AppState",user.AppState??"")
};

    // Añadir roles
    var roles = await userManager.GetRolesAsync(user);
    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var token = tokenService.GenerateToken(claims);
    return Results.Ok(new { Token = token });
});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorSeguridad2026.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
