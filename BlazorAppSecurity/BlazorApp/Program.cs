using BlazorApp.Components;
using BlazorApp.Components.Account;
using BlazorApp.Components.Account.Pages.Manage;
using BlazorApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;



var builder = WebApplication.CreateBuilder(args);





// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
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

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


builder.Services.AddHttpClient();  

//("ServerAPI",
//      client => client.BaseAddress = new Uri(@"https://localhost:7270/"));
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
//  .CreateClient("ServerAPI"));








var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();


//para que funcionen las apis//


app.MapGet("/api/free", ()=>"Hola mundo");
app.MapGet("/api/hello", [Authorize] () => "Hello world!");
app.MapGet("/api/admin", [Authorize(Roles = "Administrators")] () => "Hello administratos");
app.MapGet("/api/login/{text}", async (string text, IServiceProvider serviceProvider) =>
{
    var texto = text.Split("&");
    var  service = (SignInManager <ApplicationUser>) serviceProvider.GetService(typeof(SignInManager<ApplicationUser>));
    var result = await service.PasswordSignInAsync(texto[0], texto[1], true, lockoutOnFailure: false);
    if (result.Succeeded)
        Results.Ok();
    else
        Results.Unauthorized();

});

app.MapGet("/api/logout", async ( IServiceProvider serviceProvider) =>
{
    var signInManager = (SignInManager<ApplicationUser>)serviceProvider.GetService(typeof(SignInManager<ApplicationUser>));
    await signInManager.SignOutAsync();
});


app.Run();


static void Pepe (IServiceProvider serviceProvider)
{
    var service = serviceProvider.GetService(typeof(SignInManager<ApplicationUser>));
};
