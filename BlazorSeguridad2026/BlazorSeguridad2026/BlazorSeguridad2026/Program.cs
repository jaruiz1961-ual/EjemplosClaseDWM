using Blazored.LocalStorage;
using BlazorSeguridad2026.Components;
using BlazorSeguridad2026.Components.Account;
using BlazorSeguridad2026.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Shares.Contextos;
using Shares.Genericos;
using Shares.SeguridadToken;


var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()      // o .WithOrigins("https://tudominio.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa el token JWT de la forma: Bearer {token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IAppState, AppState>();
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


// Interceptor para multitenant (opcional)
builder.Services.AddTransient<TenantSaveChangesInterceptor>();

// Factorías de DbContexts para todos los backends locales:
builder.Services.AddDbContextFactory<SqlServerDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("SqlServerDbContext");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseSqlServer(conn);
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Transient);
builder.Services.AddDbContextFactory<SqLiteDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("SqLiteDbContext");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseSqlite(conn);
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Transient);
builder.Services.AddDbContextFactory<InMemoryDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("InMemoryDbContext");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseInMemoryDatabase(conn);
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Transient);

// Factoría genérica por entidad
builder.Services.AddScoped(typeof(IGenericRepositoryFactory<>), typeof(GenericRepositoryFactory<>));

// Factoría de UoW
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    void InitDb<TDb>(string factoryTypeKey, Action<TDb> migrator)
        where TDb : DbContext
    {
        var factory = scope.ServiceProvider.GetService<IDbContextFactory<TDb>>();
        var db = factory?.CreateDbContext();
        migrator(db);
    }

    InitDb<SqlServerDbContext>("SqlServer", db => db?.Database.Migrate());
    InitDb<SqLiteDbContext>("SqLite", db => db?.Database.Migrate());
    InitDb<InMemoryDbContext>("InMemory", db => db?.Database.EnsureCreated());
}
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});


var app = builder.Build();


// Minimal API para autenticación en APIs y generación de token JWT






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


app.UseCors(MyAllowSpecificOrigins);

app.UseRouting();              // <- IMPORTANTE: routing antes de auth
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

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
