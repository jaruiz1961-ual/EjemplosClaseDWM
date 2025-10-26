using Sosa.Reservas.API;
using Sosa.Reservas.Application;
using Sosa.Reservas.Common;
using Sosa.Reservas.External;
using Sosa.Reservas.Persistence;
using Sosa.Reservas.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de los servicios del contenedor
builder.Services.AddControllers();
builder.Services.AddOpenApi();


// Inyecci�n de dependencias
builder.Services
    .AddWebApi()
    .AddCommon()
    .AddApplication()
    .AddExternal(builder.Configuration)
    .AddPersistence(builder.Configuration);


// Configuraci�n de la autorizaci�n
builder.Services.AddAuthorization();

var app = builder.Build();

// Configuraci�n del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await IdentityDataSeeder.SeedRolesAsync(app);

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();