using Blazored.LocalStorage;
using BlazorSeguridad2026.Client.Pages;
using BlazorSeguridad2026.Components;
using BlazorSeguridad2026.Components.Account;
using BlazorSeguridad2026.Data;
using DataBase;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Security.Claims;
using Cliente;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;



// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
.AddRoles<IdentityRole>() // ← Añade esta línea
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ContextProvider>(sp =>
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

Uri DirBase = new Uri(configuration.GetConnectionString("UrlApi") ?? "https://localhost:7013/");
builder.Services.AddHttpClient("ApiRest", (sp, client) =>
{
    client.BaseAddress = DirBase;
});


var app = builder.Build();



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
