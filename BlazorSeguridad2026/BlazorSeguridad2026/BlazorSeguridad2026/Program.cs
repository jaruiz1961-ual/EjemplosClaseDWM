using BlazorAppEFTenant.Components.EndPoints;
using Blazored.LocalStorage;
using BlazorSeguridad2026.Components;
using BlazorSeguridad2026.Components.Account;
using BlazorSeguridad2026.Components.Seguridad;
using BlazorSeguridad2026.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shares.Contextos;
using Shares.Genericos;
using Shares.Modelo;
using Shares.Seguridad;
using Shares.SeguridadToken;
using static TenantInterop;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger + JWT
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

// Configuración
IConfiguration configuration = builder.Configuration;
var UrlApi = configuration["ConnectionStrings:UrlApi"] ?? "https://localhost:7013/";
var ApiName = configuration["ConnectionStrings:ApiName"] ?? "ApiRest";
var ConnectionMode = configuration["ConnectionStrings:ConnectionMode"] ?? "Ef";
var DataProvider = configuration["DataProvider"] ?? "SqlServer";
var TenantId = configuration["TenantId"] ?? "0";

var connectionString = configuration.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// =================== HttpClient ===================

// 1) HttpClient para el propio servidor Blazor (mismo host que la app + Identity)
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(nav.BaseUri) // p.ej. https://localhost:7013/
    };
});

// 2) IHttpClientFactory + cliente nombrado para API externa (UrlApi)
builder.Services.AddHttpClient(ApiName, client =>
{
    client.BaseAddress = new Uri(UrlApi); // p.ej. https://api.midominio.com/
});

// (Opcional) Si quieres inyectar directamente un HttpClient para la API externa:
builder.Services.AddScoped<HttpClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient(ApiName);
});

// =================== Blazor / Identity ===================

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<ITokenService, TokenService>(); // para JWT hacia APIs

builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();

// Local Storage
builder.Services.AddBlazoredLocalStorage();

// ContextProvider multi-tenant
builder.Services.AddScoped<ContextProvider>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var http = sp.GetRequiredService<IHttpContextAccessor>();
    var config = sp.GetRequiredService<IConfiguration>();

    var cp = new ContextProvider(localStorage, http, config)
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

// Interceptor multi-tenant
builder.Services.AddScoped<TenantSaveChangesInterceptor>();
builder.Services.AddScoped<ITenantService, TenantService>();

// DbContext factories
builder.Services.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("DefaultConnection");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseSqlServer(conn);
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Transient);

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

// Factorías genéricas / UoW
builder.Services.AddScoped(typeof(IGenericRepositoryFactoryAsync<>), typeof(GenericRepositoryFactory<>));
builder.Services.AddScoped(typeof(IUnitOfWorkAsync), typeof(UnitOfWorkAsync));
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});

var app = builder.Build();

// ServiceLocator para NavMenu
ServiceLocator.Services = app.Services;

// Migraciones
using (var scope = app.Services.CreateScope())
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

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorSeguridad2026.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

// Tus Minimal APIs
app.GenericApis<Usuario>();
app.LoginApis();

app.Run();
