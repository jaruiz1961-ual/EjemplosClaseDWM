using BlazorAppEFTenant.Components.EndPoints;
using Blazored.LocalStorage;
using BlazorSeguridad2026.Components;
using BlazorSeguridad2026.Components.Account;
using BlazorSeguridad2026.Components.Seguridad;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shares.Contextos;
using Shares.Genericos;
using BlazorSeguridad2026.Data;
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

var applicationConnectionString = configuration.GetConnectionString("ApplicationDbContext")
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.");

// Razor Components / Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

// Autenticación/autorización (Identity + roles)
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

// DbContext Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(applicationConnectionString, sql =>
    {
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,                     // nº de reintentos
            maxRetryDelay: TimeSpan.FromSeconds(10), // retraso máx. entre reintentos
            errorNumbersToAdd: null);             // o lista de códigos de error específicos
    }));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity con ApplicationUser / ApplicationRole (int)
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Servicios de seguridad
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<ITokenService, TokenService>();

// Local Storage
builder.Services.AddBlazoredLocalStorage();

// ContextProvider multitenant
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

// HttpClient hacia la API
builder.Services.AddHttpClient(ApiName, (sp, client) =>
{
    client.BaseAddress = new Uri(UrlApi);
});





// Interceptor multitenant
builder.Services.AddTransient<TenantSaveChangesInterceptor>();

// Factorías de DbContexts para backends locales
builder.Services.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("ApplicationDbContext");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseSqlServer(conn);
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Transient);

// Factorías de DbContexts para backends locales
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

// Factoría genérica y UoW
builder.Services.AddScoped(typeof(IGenericRepositoryFactoryAsync<>), typeof(GenericRepositoryFactory<>));
builder.Services.AddScoped(typeof(IUnitOfWorkAsync), typeof(UnitOfWorkAsync));
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

// Blazor Server
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});

builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Service locator (para JS/TenantInterop)
ServiceLocator.Services = app.Services;

// Migrar/crear bases de datos usando el ServiceProvider real
using (var scope = app.Services.CreateScope())
{
    void InitDb<TDb>(Action<TDb> migrator)
        where TDb : DbContext
    {
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TDb>>();
        using var db = factory.CreateDbContext();
        migrator(db);
    }

    //InitDb<SqlServerDbContext>(db => db.Database.Migrate());
    //InitDb<SqLiteDbContext>(db => db.Database.Migrate());
    //InitDb<InMemoryDbContext>(db => db.Database.EnsureCreated());
}

// Pipeline HTTP
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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorSeguridad2026.Client._Imports).Assembly);

// Endpoints de /Account de Identity
app.MapAdditionalIdentityEndpoints();

// Minimal APIs
app.GenericApis<Usuario>();
app.LoginApis();

app.Run();
