using Hospital.Auth.Core.Implementation;
using Hospital.Auth.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

if (builder.Environment.IsEnvironment("Testing"))
    builder.Services.AddMockAuthServicesForTesting();
else
    builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build();

app.MapEndpoints();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
