using BlazorAppEFTenant.Components;
using BlazorAppEFTenant.Components.EndPoints;
using Blazored.LocalStorage;
using DataBase;
using DataBase.Contextos;
using DataBase.Genericos;
using DataBase.Modelo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

//JWt
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Agrega los servicios CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {

                          policy.WithOrigins("https://localhost:7013") // <- Ajusta ESTOS puertos
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials(); // Importante para las cookies
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

builder.Services.AddScoped<IAppState,AppState>();


//builder.Services.AddScoped<IAppState>(sp =>
//{
//    return new AppState();
//});

// Registrar TokenService en DI
builder.Services.AddSingleton<ITokenService,TokenService>();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();



// Configuración de Razor y componentes Blazor (igual que en tu código original)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();



// HttpClient para APIs externas (AJUSTA la clave según tu appsettings)
Uri DirBase = new Uri(configuration.GetConnectionString("UrlApi") ?? "https://localhost:7013/");
builder.Services.AddHttpClient("ApiRest", (sp, client) =>
{
    client.BaseAddress = DirBase;
});

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ContextProvider>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var token = sp.GetRequiredService<ITokenService>();

    var cp = new ContextProvider(localStorage, token)
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

// Ambas interfaces apuntan a la MISMA instancia
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IContextProvider>(sp => sp.GetRequiredService<ContextProvider>());
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ContextProvider>());

builder.Services.AddAuthorizationCore();


// Tenant Provider y Context Provider




//(sp =>
//{
//    var provider = ActivatorUtilities.CreateInstance<ContextProvider>(sp);
//   // provider.SetContext(1, "InMemory", "ApiRest", new Uri(@"https://localhost:7013/"), "Ef",null); // Asigna aquí el valor inicial por defecto
//    return provider;
//});

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
},ServiceLifetime.Transient);
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


// OJO: Si tu UnitOfWork generic depende solo de DI-resolvable (DbContext y factory)
//builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWorkEf<>));

// Inicialización/Migración de BD al arrancar
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
app.UseCors(MyAllowSpecificOrigins);

app.UseRouting();              // <- IMPORTANTE: routing antes de auth
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
    .AddAdditionalAssemblies(typeof(BlazorAppEFTenant.Client._Imports).Assembly);

app.GenericApis<Usuario>();//mapeo a APIS

app.Run();
