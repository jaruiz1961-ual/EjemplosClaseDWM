using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("todos")
?? "Data Source=todos.db";


builder.Services.AddRazorPages();

builder.Services.AddDbContext<TodoDb>(options => options.UseSqlite(connectionString)).AddSqlite<TodoDb>(connectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

 builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddAuthorization();



var app = builder.Build();

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


