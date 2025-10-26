var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


//app.MapGet("/", () => "Hello World!");
app.MapGet("/hello", () => "Hello World!");
app.MapGet("/todos", () => new { TodoItem = "Learn about routing", Complete = false });
app.MapGet("/hello/{name:int}", (string name) => $"Hello {name}");

app.MapRazorPages();

app.Run();
