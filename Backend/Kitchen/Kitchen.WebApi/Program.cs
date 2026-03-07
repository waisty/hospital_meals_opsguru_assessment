using Hospital.Kitchen.Core;
using Hospital.Kitchen.WebApi;
using Hospital.Kitchen.WebApi.Authentication;
using Hospital.Kitchen.WebApi.EndpointMappings;

var builder = WebApplication.CreateBuilder(args);

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
    builder.Services.AddKitchenServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
}

var app = builder.Build();

if (allowedOrigins.Length > 0)
    app.UseCors();
app.UseJwtAuthentication();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Kitchen.WebApi", status = "running" }));
var api = app.MapGroup("/api/v1");
api.MapKitchenEndpointMappings();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
