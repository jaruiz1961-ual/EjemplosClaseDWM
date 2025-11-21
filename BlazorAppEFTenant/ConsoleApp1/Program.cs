// See https://aka.ms/new-console-template for more information
using System;

Console.WriteLine("Hello, World!");
using var scope = app.Services.CreateScope();
var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // o SqlDbContext real
bool ok = await ctx.Database.CanConnectAsync();
Console.WriteLine("CanConnect = " + ok);
