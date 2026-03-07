using Hospital.Meals.Core;
using Hospital.Meals.WebApi;
using Hospital.Meals.WebApi.Authentication;
using Hospital.Meals.WebApi.EndpointMappings;
using Hospital.Meals.WebApi.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? [];
if (allowedOrigins.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
        });
    });
}

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMealsServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
}

var app = builder.Build();

if (allowedOrigins.Length > 0)
    app.UseCors();
app.UseJwtAuthentication();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Meals.WebApi", status = "running" }));
var api = app.MapGroup("/api/v1").AddEndpointFilter<ValidationEndpointFilter>();
api.MapMealEndpointMappings();
api.MapRecipeEndpointMappings();
api.MapIngredientEndpointMappings();
api.MapPatientRequestEndpointMappings();
api.MapReferenceDataEndpointMappings();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
