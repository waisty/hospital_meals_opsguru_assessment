using Hospital.Auth.Core.Implementation;
using Hospital.Auth.WebApi;
using Hospital.Auth.WebApi.EndpointMappings;

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
    builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build();

if (allowedOrigins.Length > 0)
    app.UseCors();
app.MapGet("/", () => Results.Ok(new { service = "Hospital.Auth.WebApi", status = "running" }));
var api = app.MapGroup("/api/v1");
api.MapAuthEndpointMappings();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
