using Hospital.Kitchen.Core;
using Hospital.Kitchen.WebApi;
using Hospital.Kitchen.WebApi.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKitchenServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseJwtAuthentication();

app.MapEndpoints();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
