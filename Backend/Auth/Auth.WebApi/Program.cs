using Hospital.Auth.Core.Implementation;
using Hospital.Auth.WebApi;
using Hospital.Auth.WebApi.EndpointMappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

if (!builder.Environment.IsEnvironment("Testing"))
    builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Auth.WebApi", status = "running" }));
var api = app.MapGroup("/api/v1");
api.MapAuthEndpointMappings();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
