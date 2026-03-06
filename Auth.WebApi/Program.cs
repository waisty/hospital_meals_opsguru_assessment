using Hospital.Auth.Core.Contracts;
using Hospital.Auth.Core.Implementation;
using Hospital.Auth.UIViewModels;
using WebExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("Testing"))
    builder.Services.AddMockAuthServicesForTesting();
else
    builder.Services.AddAuthServices(builder.Configuration); // includes automatic EF Core migration at startup

// Add services to the container.

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Auth.WebApi", status = "running" }));

// Configure the HTTP request pipeline.

app.MapPost("/login", async (UserAuthRequest request, IAuthHandler authHandler) =>
{
    var response = await authHandler.AuthenticateUserAsync(request);
    return response is null ? Results.Unauthorized() : Results.Ok(response);
});

//app.MapPost("/get-users", () =>
//{

//}).RequireAuthorization();

//app.MapPost("/get-users", () =>
//{

//}).RequireAuthorization();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }

