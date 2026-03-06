using Hospital.Meals.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMealsServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Meals.WebApi", status = "running" }));

app.Run();
