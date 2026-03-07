using Hospital.Meals.Core;
using Hospital.Meals.WebApi;
using Hospital.Meals.WebApi.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMealsServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
}

var app = builder.Build();

app.UseJwtAuthentication();

app.MapEndpoints();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
