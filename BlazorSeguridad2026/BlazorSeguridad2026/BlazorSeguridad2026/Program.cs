using BibliotecaContextosDb.Genericos;
using BlazorAppEFTenant.Components.EndPoints;
using Blazored.LocalStorage;
using BlazorSeguridad2026.Base.Contextos;
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Components;
using BlazorSeguridad2026.Components.Account;
using BlazorSeguridad2026.Data.Modelo;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.OpenApi.Models;
using BlazorSeguridad2026.Base.Culture;

//using static TenantInterop;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");




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
builder.Services.AddDbContext<ApplicationBaseDbContext>(options =>
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
    .AddEntityFrameworkStores<ApplicationBaseDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Servicios de seguridad
builder.Services.AddScoped<IRoleService, RoleServiceMio>();
builder.Services.AddScoped<IUserService, UserServiceMio>();
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

//http minimal APIs y Blazor



// Interceptor multitenant
builder.Services.AddTransient<TenantSaveChangesInterceptor>();

// Factorías de DbContexts para backends locales
builder.Services.AddDbContextFactory<ApplicationBaseDbContext>((sp, options) =>
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

builder.Services.AddDbContextFactory<InMemoryBaseDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("InMemoryDbContext");
    var interceptor = sp.GetRequiredService<TenantSaveChangesInterceptor>();
    options.UseInMemoryDatabase(conn);
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Transient);




//IDictionary<string, IDbContextFactory<DbContext>>
//necesito crear la factoria para quitar el switch de UnitOfWorkFactory
builder.Services.AddScoped<IMultiDbContextFactory>(sp =>
{
    var map = new Dictionary<string, Func<DbContext>>
    {
        ["application"] = () =>
            sp.GetRequiredService<IDbContextFactory<ApplicationBaseDbContext>>()
              .CreateDbContext(),

        ["sqlserver"] = () =>
            sp.GetRequiredService<IDbContextFactory<SqlServerDbContext>>()
              .CreateDbContext(),

        ["sqlite"] = () =>
            sp.GetRequiredService<IDbContextFactory<SqLiteDbContext>>()
              .CreateDbContext(),

        ["inmemory"] = () =>
            sp.GetRequiredService<IDbContextFactory<InMemoryBaseDbContext>>()
              .CreateDbContext()
    };

    return new MultiDbContextFactory(map);
});


// Estrategias por contexto (una vez, para todos los TEntity)
builder.Services.AddScoped<IRepositoryContextStrategy, ApplicationRepositoryStrategy>();
builder.Services.AddScoped<IRepositoryContextStrategy, SqlServerRepositoryStrategy>();
builder.Services.AddScoped<IRepositoryContextStrategy, SqLiteRepositoryStrategy>();
builder.Services.AddScoped<IRepositoryContextStrategy, InMemoryRepositoryStrategy>();

// Factoría genérica de repositorios
builder.Services.AddScoped(typeof(IGenericRepositoryFactoryAsync<>), typeof(GenericRepositoryFactory<>));



// Factoría genérica y UoW
builder.Services.AddScoped(typeof(IGenericRepositoryFactoryAsync<>), typeof(GenericRepositoryFactory<>));
builder.Services.AddScoped(typeof(IUnitOfWorkAsync), typeof(UnitOfWorkAsync));
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();



builder.Services.AddRazorPages();

// Blazor Server
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});
builder.Services.AddLocalization();

builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();

builder.Services.AddBootstrapBlazor();


var app = builder.Build();

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(SupportedCultures.DefaultCulture),
    SupportedCultures = SupportedCultures.Cultures,
    SupportedUICultures = SupportedCultures.Cultures
};



app.UseRequestLocalization(localizationOptions);

// Service locator (para JS/TenantInterop)
//ServiceLocator.Services = app.Services;

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

app.UseStaticFiles();
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
