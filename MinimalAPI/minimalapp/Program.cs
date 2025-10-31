using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimalapp;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Agrega los servicios CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                      
                          policy.WithOrigins("https://localhost:7041", "http://localhost:5090") // <- Ajusta ESTOS puertos
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials(); // Importante para las cookies
                      });
});

// ... resto de servicios

var connectionString = builder.Configuration.GetConnectionString("todos")
    ?? "Data Source=todos.db";

builder.Services.AddRazorPages();

builder.Services.AddDbContext<TodoDb>(options => options.UseSqlite(connectionString)).AddSqlite<TodoDb>(connectionString);

builder.Services.AddEndpointsApiExplorer();

// Swagger con esquema Bearer HTTP (mejor experiencia en UI)
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

// Registrar TokenService en DI
builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = null;
        options.TokenValidationParameters = new ()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
        
    });

builder.Services.AddAuthorization();

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

// Minimal API endpoints
app.MapGet("/hello", () => "Hello World!");
app.MapGet("/todo", () => new { TodoItem = "Learn about routing", Complete = false });
app.MapGet("/hello/{name}", (string name) => $"Hello {name}");

// Exponer Razor Pages (.cshtml)
app.MapRazorPages();

app.MapGet("/todos", async (TodoDb db) => await db.Todos.ToListAsync());
app.MapPost("/todos", async (TodoDb db, TodoItem todo) =>
{
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todo/{todo.Id}", todo);
});

app.MapGet("/todos/{id}", async (TodoDb db, int id) => await db.Todos.FindAsync(id));

app.MapPut("/todos/{id}", async (TodoDb db, TodoItem updateTodo, int id) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    todo.Item = updateTodo.Item;
    todo.IsComplete = updateTodo.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todos/{id}", async (TodoDb db, int id) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("secured-route", () => "Hello, you are authorized to see this!").RequireAuthorization();

// Login: recibir DTO desde el body y TokenService desde DI ([FromServices])
app.MapPost("/login", ([FromBody] string login, [FromServices] TokenService tokenService , HttpContext httpContext)=>
{
    if (login != "admin") return Results.Unauthorized();

    var token = tokenService.GenerateToken(login);

    var cookieOptions = new CookieOptions
    {
        // 🚨 IMPORTANTE: HttpOnly = false permite que JS (Blazor) acceda.
        // Esto es lo que solicitaste, pero es MENOS seguro.
        HttpOnly = false,
        Secure = true, // Debe ser 'true' si usas HTTPS (recomendado)
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.AddHours(1) // La cookie expira en 1 hora
    };

    // 4. Agregar la Cookie a la respuesta HTTP
    // El nombre de la cookie ("jwt_auth_token") debe coincidir con el usado en Blazor (CookieService.cs).
    httpContext.Response.Cookies.Append("jwt_auth_token", token,cookieOptions);
    return Results.Ok(new { Token = token });
});

app.Run();

class TodoItem
{
    public int Id { get; set; }
    public string? Item { get; set; }
    public bool IsComplete { get; set; }
}
class TodoDb : DbContext
{
    public TodoDb(DbContextOptions options) : base(options)
    { }
    public DbSet<TodoItem> Todos { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Todos");
    }
}

// DTO simple para login
class LoginRequest
{
    public string? Username { get; set; }
}


//curl - s - X POST http://localhost:5063/login -H "Content-Type: application/json" -d "{\"username\":\"admin\"}" -o response.json
//for / f "usebackq delims=" % T in (`powershell - NoProfile - Command "(Get-Content response.json | ConvertFrom-Json).Token"`) do set TOKEN =% T
//curl - i http://localhost:5063/secured-route -H "Authorization: Bearer %TOKEN%"


//•	Si usas HTTPS local con certificado dev, añade -k a curl o usa Invoke-RestMethod -SkipCertificateCheck en entornos de prueba.

