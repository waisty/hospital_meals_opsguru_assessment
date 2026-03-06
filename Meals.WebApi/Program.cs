using Hospital.Meals.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMealsServices(builder.Configuration); // includes EF Core migration and seed data at startup

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Meals.WebApi", status = "running" }));

app.Run();
