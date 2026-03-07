using Hospital.Kitchen.Core;
using Hospital.Kitchen.WebApi;
using Hospital.Kitchen.WebApi.Authentication;
using Hospital.Kitchen.WebApi.EndpointMappings;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddKitchenServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
}

var app = builder.Build();

app.UseJwtAuthentication();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Kitchen.WebApi", status = "running" }));
var api = app.MapGroup("/api/v1");
api.MapKitchenEndpointMappings();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
